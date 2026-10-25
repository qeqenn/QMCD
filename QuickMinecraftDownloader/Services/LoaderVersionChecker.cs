using Optifine.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuickMinecraftDownloader.Services
{
    public static class LoaderVersionChecker
    {
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        private static async Task<string> GetStringWithRetryAsync(
    string url, int maxRetries = 2, CancellationToken token = default)
        {
            Exception lastEx = null;

            for (int i = 0; i < maxRetries; i++)
            {
                // 单源 8 秒超时，快速失败换下一个
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(TimeSpan.FromSeconds(8));

                try
                {
                    System.Diagnostics.Debug.WriteLine($"[Fallback] 尝试：{url}（第 {i + 1} 次）");
                    return await _http.GetStringAsync(url, cts.Token);
                }
                catch (TaskCanceledException) when (!token.IsCancellationRequested)
                {
                    // 超时：不重试，直接换下一个源
                    lastEx = new TimeoutException($"请求超时（8s）：{url}");
                    System.Diagnostics.Debug.WriteLine($"[Fallback] 超时：{url}");
                    break;
                }
                catch (HttpRequestException ex) when (i < maxRetries - 1)
                {
                    lastEx = ex;
                    System.Diagnostics.Debug.WriteLine($"[Fallback] 失败：{url} | {ex.Message}");
                    await Task.Delay(500 * (1 << i), token);
                }
            }

            throw lastEx ?? new HttpRequestException($"请求失败：{url}");
        }

        private static async Task<string> GetStringWithFallbackAsync(
    IEnumerable<string> urls, CancellationToken token = default)
        {
            var errors = new List<string>();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var url in urls)
            {
                try
                {
                    string result = await GetStringWithRetryAsync(url, maxRetries: 2, token);

                    // HTML 校验
                    string trimmed = result.TrimStart();
                    if (trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidDataException($"返回 HTML 而非数据：{url}");
                    }

                    System.Diagnostics.Debug.WriteLine(
                        $"[Fallback] 成功：{url}（总耗时 {sw.ElapsedMilliseconds}ms）");
                    return result;
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;   // 用户主动取消，不当作失败
                }
                catch (Exception ex)
                {
                    errors.Add($"{url} → {ex.Message}");
                    System.Diagnostics.Debug.WriteLine(
                        $"[Fallback] 失败：{url} | {ex.Message}（累计 {sw.ElapsedMilliseconds}ms）");
                }
            }

            throw new HttpRequestException(
                "所有源均不可用：\n" + string.Join("\n", errors));
        }

        /// <summary>获取四个加载器针对指定 MC 版本的可用版本列表</summary>
        public static async Task<Dictionary<string, List<string>>> GetAllAsync(string mcVersion)
        {
            var results = new Dictionary<string, List<string>>
            {
                ["Forge"] = new(),
                ["NeoForge"] = new(),
                ["Fabric"] = new(),
                ["Optifine"] = new()
            };

            var tasks = new (string key, Task<List<string>> task)[]
            {
        ("Forge",    GetForgeVersionsAsync(mcVersion)),
        ("NeoForge", GetNeoForgeVersionsAsync(mcVersion)),   // ← 必须有这行
        ("Fabric",   GetFabricVersionsAsync(mcVersion)),
        ("Optifine", GetOptifineVersionsAsync(mcVersion))
            };

            foreach (var (key, task) in tasks)
            {
                try
                {
                    results[key] = await task;
                    System.Diagnostics.Debug.WriteLine($"[GetAll] {key} 查到 {results[key].Count} 个版本");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetAll] {key} 失败：{ex.Message}");
                    results[key] = new List<string>();
                }
            }

            return results;
        }

        // ---------- Forge：精确前缀匹配 ----------
        private static async Task<List<string>> GetForgeVersionsAsync(string mcVersion)
        {
            var urls = new[]
            {
        "https://files.minecraftforge.net/net/minecraftforge/forge/maven-metadata.json",
        "https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/maven-metadata.json",
        "https://bsmirror.hkmc.online/maven/net/minecraftforge/forge/maven-metadata.json"
    };

            string json = await GetStringWithFallbackAsync(urls);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty(mcVersion, out var arr))
                return new List<string>();

            return arr.EnumerateArray()
                .Select(x => x.GetString())
                .Where(v => !string.IsNullOrEmpty(v))
                .OrderByDescending(v => v, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // ---------- NeoForge：按数字段精确匹配 ----------
        private static async Task<List<string>> GetNeoForgeVersionsAsync(string mcVersion)
        {
            try
            {
                var urls = new[]
{
    "https://neoforged.forgecdn.net/releases/net/neoforged/neoforge/maven-metadata.xml",
    "https://maven.neoforged.net/releases/net/neoforged/neoforge/maven-metadata.xml",
    "https://bmclapi2.bangbang93.com/maven/net/neoforged/neoforge/maven-metadata.xml"
};

                string xml = await GetStringWithFallbackAsync(urls);
                var doc = XDocument.Parse(xml);
                var allVersions = doc.Descendants("version").Select(e => e.Value).ToList();
                var matched = new List<string>();
                var mcParts = mcVersion.Split('.');

                // 分支 A：处理 Minecraft 1.x 旧格式 (对应 NeoForge 旧版命名规则)
                if (mcParts.Length >= 2 && mcParts[0] == "1")
                {
                    // 旧规则：MC 1.20.2 -> NeoForge 以 "20.2." 开头
                    string expectedPrefix = $"{mcParts[1]}.{mcParts[2]}.";

                    matched = allVersions
                        .Where(v => v.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                // 分支 B：处理 Minecraft 26.x+ 新格式 (对应 NeoForge 新版命名规则)
                else if (mcParts.Length >= 2 && int.TryParse(mcParts[0], out _))
                {
                    // 新规则：MC 26.1 -> NeoForge 以 "26.1." 开头
                    // 前三位 (年.季.热修) 对应 MC 版本，因此我们用 "年.季." 作为前缀。
                    // 如果 MC 版本自带热修号 (如 26.1.1)，则要求更精确的 "26.1.1." 前缀。
                    string expectedPrefix = mcParts.Length >= 3
                        ? $"{mcParts[0]}.{mcParts[1]}.{mcParts[2]}."  // MC 26.1.1 -> 匹配 "26.1.1."
                        : $"{mcParts[0]}.{mcParts[1]}.";              // MC 26.1 -> 匹配 "26.1."

                    matched = allVersions
                        .Where(v => v.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // 统一排序：版本号高的在前
                return matched
                    .OrderByDescending(v => v, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NeoForge] 查询失败：{ex.Message}");
                return new List<string>();
            }
        }

        // ---------- Fabric：官方 JSON API ----------
        private static async Task<List<string>> GetFabricVersionsAsync(string mcVersion)
        {
            try
            {
                string apiPath = $"v2/versions/loader/{Uri.EscapeDataString(mcVersion)}/";
                var urls = new[]
                {
        $"https://meta.fabricmc.net/{apiPath}",
        $"https://bmclapi2.bangbang93.com/fabric-meta/{apiPath}"
    };

                string json = await GetStringWithFallbackAsync(urls);
                using var doc = JsonDocument.Parse(json);

                // 返回数组，每项形如 { "loader": { "version": "0.15.7", ... } }
                var list = doc.RootElement.EnumerateArray()
                    .Select(x =>
                    {
                        if (x.TryGetProperty("loader", out var loader) &&
                            loader.TryGetProperty("version", out var ver))
                            return ver.GetString();
                        return null;
                    })
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();

                return list;
            }
            catch { return new List<string>(); }
        }

        // ---------- OptiFine：BMCLAPI 镜像 ----------
        private static async Task<List<string>> GetOptifineVersionsAsync(string mcVersion)
        {
            try
            {
                var installer = new OptifineInstaller(new HttpClient());
                var allVersions = await installer.GetOptifineVersionsAsync();

                // 过滤出属于当前 Minecraft 版本的 OptiFine
                // 注意：库里的 Version 字符串可能不包含 "OptiFine_" 前缀，具体看库返回什么
                // 通常是 "1.20.4_HD_U_I7" 或者 "preview_1.20.4_HD_U_I7"
                var matched = allVersions
                    .Where(v => v.Version.Contains(mcVersion, StringComparison.OrdinalIgnoreCase))
                    .Select(v => v.Version)
                    .OrderByDescending(v => v)
                    .ToList();

                return matched;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Optifine] 查询失败: {ex.Message}");
                return new List<string>();
            }
        }
    }
}