using QuickMinecraftDownloader.Models;
using QuickMinecraftDownloader.Services;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;
//using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Optifine.Installer;


namespace QuickMinecraftDownloader
{
    public partial class Form1 : Form
    {
        private JsonRepository<MemoryItem> _db;
        private List<VersionEntry> _cachedVersions;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
                string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "QuickMinecraftDownloader", "memories.json");
                _db = new JsonRepository<MemoryItem>(dbPath, x => x.Id);

                //查询上一次的文件夹
                var history = _db.Query(x => x.Name == "LastFolder")
                         .OrderByDescending(x => x.CreatedAt)
                         .ToList();

                // 去重：同一个路径只保留最近一条
                history = history
                    .GroupBy(x => x.Value)
                    .Select(g => g.First())
                    .ToList();

                // 只把存在的文件夹放进下拉框
                var paths = history
                    .Where(x => Directory.Exists(x.Value))
                    .Select(x => x.Value)
                    .ToList();

                输出文件夹.DataSource = null;          // 先解绑
                输出文件夹.DataSource = paths;

                // 默认选中最近的一条
                if (paths.Count > 0)
                    输出文件夹.SelectedIndex = 0;

                // 刷新版本列表
                刷新GameVer_Click(null, null);

                ResetLoaderRadios();
            
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 选择文件夹_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            // 2. 设置对话框属性
            dialog.Description = "选择输出文件夹"; // 提示文字
            dialog.RootFolder = Environment.SpecialFolder.MyComputer; // 设置起始根目录
            dialog.SelectedPath = @".minecraft"; // 设置默认选中的路径
            dialog.ShowNewFolderButton = true; // 是否显示"新建文件夹"按钮（默认为true）

            // 3. 显示对话框并判断用户操作
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 4. 获取用户选择的文件夹路径
                string selectedPath = dialog.SelectedPath;

                // 将路径显示在文本框
                输出文件夹.Text = selectedPath;
            }

        }

        private async void 扫描文件夹_Click(object sender, EventArgs e)
        {
            //扫描.minecraft

            string keyword = ".minecraft";

            var drives = DriveScanner.GetAllDrives();
            if (drives.Count == 0)
            {
                MessageBox.Show("没有可扫描的磁盘。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            扫描文件夹.Enabled = 开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;

            // 用 Items 模式：全程不设 DataSource
            输出文件夹.DataSource = null;
            输出文件夹.Items.Clear();
            输出文件夹.Items.Add($"扫描中：{string.Join(", ", drives)}");
            输出文件夹.SelectedIndex = 0;

            try
            {
                // 每个盘一个后台任务，并行跑
                var tasks = drives.Select(drive => Task.Run(() =>
                    FolderScanner.Scan(drive, keyword, maxDepth: 6, maxResults: 200)))
                    .ToArray();

                var resultsPerDrive = await Task.WhenAll(tasks);

                // 合并 → 去重 → 排序
                var allPaths = resultsPerDrive
                    .SelectMany(x => x)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 刷新 ComboBox
                输出文件夹.Items.Clear();
                if (allPaths.Count == 0)
                {
                    MessageBox.Show("没有找到MC游戏文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    foreach (var p in allPaths)
                        输出文件夹.Items.Add(p);

                    输出文件夹.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("扫描出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                输出文件夹.DropDownStyle = ComboBoxStyle.DropDownList;
                扫描文件夹.Enabled = 开始安装.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void 刷新GameVer_Click(object sender, EventArgs e)
        {
            刷新GameVer.Enabled = 开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;

            GameVers.DataSource = null;
            GameVers.Items.Clear();
            //GameVers.Items.Add("正在获取版本列表...");

            try
            {
                var manifest = await VersionManifestService.GetAsync();
                _cachedVersions = manifest.Versions;
                ApplyVersionFilter();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"网络请求失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON 解析失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GameVers.DropDownStyle = ComboBoxStyle.DropDownList;
                刷新GameVer.Enabled = 开始安装.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
        private void ApplyVersionFilter()
        {
            if (_cachedVersions == null) return;

            IEnumerable<VersionEntry> filtered;

            if (正式版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "release");
            }
            else if (预览版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "snapshot" && !IsAprilFools(v));
            }
            else if (愚人节版.Checked)
            {
                filtered = _cachedVersions.Where(v => IsAprilFools(v));
            }
            else if (远古版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "old_alpha" || v.Type == "old_beta");
            }
            else
            {
                filtered = _cachedVersions.Where(v => v.Type == "release");
            }

            var ids = filtered.Select(v => v.Id).ToList();

            GameVers.DataSource = null;
            GameVers.Items.Clear();

            if (ids.Count == 0)
            {
                GameVers.Items.Add("未找到匹配版本");
                return;
            }

            foreach (var id in ids)
                GameVers.Items.Add(id);

            GameVers.SelectedIndex = 0;
        }
        private static bool IsAprilFools(VersionEntry v)
        {
            if (string.IsNullOrEmpty(v.ReleaseTime)) return false;
            if (!DateTimeOffset.TryParse(v.ReleaseTime, out var dto)) return false;

            var utc = dto.UtcDateTime;
            return utc.Month == 4 && utc.Day == 1;
        }

        private void 正式版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 预览版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 愚人节版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 远古版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        /// <summary>判断路径最后一级目录名是否为 .minecraft（忽略大小写）</summary>
        private static bool IsMinecraftFolder(string path)
        {
            string trimmed = path.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            string name = Path.GetFileName(trimmed);
            return string.Equals(name, ".minecraft", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回真正要写入的 .minecraft 路径：
        /// 选中目录本身就是 .minecraft → 直接用；
        /// 否则在其下创建 .minecraft。
        /// </summary>
        private static string EnsureMinecraftDir(string selectedFolder)
        {
            if (!Directory.Exists(selectedFolder))
                Directory.CreateDirectory(selectedFolder);

            if (IsMinecraftFolder(selectedFolder))
            {
                return selectedFolder.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);
            }

            string mcDir = Path.Combine(selectedFolder, ".minecraft");
            Directory.CreateDirectory(mcDir);
            return mcDir;
        }

        private async void 开始安装_Click(object sender, EventArgs e)
        {
            // ---- 校验（不变）----
            if (GameVers.SelectedItem == null ||
                GameVers.SelectedItem.ToString() == "未找到匹配版本")
            {
                MessageBox.Show("请先选择游戏版本。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string versionId = GameVers.SelectedItem.ToString();

            string selectedFolder = 输出文件夹.Text?.Trim();
            if (string.IsNullOrEmpty(selectedFolder))
            {
                MessageBox.Show("请先选择输出文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var entry = _cachedVersions?.FirstOrDefault(v => v.Id == versionId);
            if (entry == null)
            {
                MessageBox.Show("未找到版本信息，请先点击「刷新游戏版本」。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool useLoader = !无加载器.Checked;
            string loaderVersion = null;
            if (useLoader)
            {
                if (加载器版本.SelectedItem == null ||
                    加载器版本.SelectedItem.ToString() == "无可用版本")
                {
                    MessageBox.Show("请先选择加载器版本。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                loaderVersion = 加载器版本.SelectedItem.ToString();
            }

            // ---- 准备 UI ----
            开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;
            安装进度.Style = ProgressBarStyle.Blocks;
            安装进度.Value = 0;

            try
            {
                string mcDir = EnsureMinecraftDir(selectedFolder);

                // ---- 阶段 1：原版下载，0~70% ----
                await InstallVersionAsync(entry, mcDir, (percent, msg) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        int v = (int)(Math.Max(0, Math.Min(100, percent)) * 0.7);
                        安装进度.Value = v;
                    }));
                });

                // ---- 阶段 2：加载器安装，70~100% ----
                if (useLoader)
                {
                    string loaderName = GetSelectedLoaderName();

                    // 加载器 installer 下载映射到 70~90
                    const int basePercent = 70;
                    const int downloadRange = 20;   // 70 -> 90

                    await InstallLoaderAsync(loaderName, versionId, loaderVersion, mcDir,
                        (style, percent, msg) =>
                        {
                            this.Invoke(new Action(() =>
                            {
                                安装进度.Style = style;
                                if (style == ProgressBarStyle.Blocks)
                                {
                                    int v = basePercent + (int)(Math.Max(0, Math.Min(100, percent)) * downloadRange / 100.0);
                                    安装进度.Value = Math.Max(0, Math.Min(100, v));
                                }
                                // 如果有状态 Label，可在此更新 msg
                            }));
                        });
                }

                // ---- 完成 ----
                安装进度.Style = ProgressBarStyle.Blocks;
                安装进度.Value = 100;

                string suffix = useLoader ? $" + {GetSelectedLoaderName()} {loaderVersion}" : "";
                MessageBox.Show($"版本 {versionId}{suffix} 安装完成！\n位置：{mcDir}", "完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装失败：" + ex.Message, "错误");
                // 👇 添加这两行，让进度条归零并恢复块状样式
                安装进度.Style = ProgressBarStyle.Blocks;
                安装进度.Value = 0;
            }
            finally
            {
                开始安装.Enabled = true;
                Cursor = Cursors.Default;

                

                SaveLastFolder();
            }
        }

        /// <summary>
        /// 把当前输出文件夹保存到记忆库：
        /// 已存在则更新时间（让它排到最前），不存在则新增。
        /// </summary>
        private void SaveLastFolder()
        {
            string folderPath = 输出文件夹.Text?.Trim();
            if (string.IsNullOrEmpty(folderPath)) return;

            var existing = _db.Query(x => x.Name == "LastFolder" && x.Value == folderPath)
                              .FirstOrDefault();

            if (existing != null)
            {
                // 已存在：只更新时间，让它排到最前
                existing.CreatedAt = DateTime.Now;
                _db.Save(existing);
            }
            else
            {
                _db.Add(new MemoryItem
                {
                    Name = "LastFolder",
                    Value = folderPath
                });
            }
        }
        private string GetSelectedLoaderName()
        {
            if (Forge.Checked) return "Forge";
            if (NeoForge.Checked) return "NeoForge";
            if (Fabric.Checked) return "Fabric";
            if (Optifine.Checked) return "Optifine";
            return null;
        }


        //安装逻辑
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };


        private async Task DownloadFileWithFallbackAsync(
    string[] urls, string savePath, Action<int> progress = null)
        {
            Exception lastEx = null;

            foreach (var url in urls)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[Download] 尝试：{url}");
                    await DownloadFileAsync(url, savePath, progress);
                    System.Diagnostics.Debug.WriteLine($"[Download] 成功：{url}");
                    return; // 下载成功就退出
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    lastEx = ex;
                    System.Diagnostics.Debug.WriteLine($"[Download] 失败：{url} | {ex.Message}");
                    // 如果文件已经创建了一部分，删掉重试
                    if (File.Exists(savePath))
                    {
                        try { File.Delete(savePath); } catch { }
                    }
                }
            }

            throw new Exception($"所有下载源均失败，最后一次错误：{lastEx?.Message}");
        }

        /// <summary>
        /// 下载指定版本到 .minecraft 目录。
        /// </summary>
        /// <param name="entry">版本清单里的条目（含 version json 的 URL）</param>
        /// <param name="mcDir">规范化后的 .minecraft 路径</param>
        /// <param name="report">进度回调（在后台线程调用）</param>
        /// <param name="report">进度回调 (百分比, 文字)，在后台线程调用</param>
        private async Task InstallVersionAsync(
            VersionEntry entry, string mcDir, Action<int, string> report)
        {
            string versionDir = Path.Combine(mcDir, "versions", entry.Id);
            Directory.CreateDirectory(versionDir);

            // ---- 阶段 1：version json（占 0~10%）----
            report(0, $"下载 {entry.Id}.json ...");
            string jsonPath = Path.Combine(versionDir, entry.Id + ".json");
            string versionJson = await _http.GetStringAsync(entry.Url);
            File.WriteAllText(jsonPath, versionJson);
            report(10, $"{entry.Id}.json 下载完成");

            // ---- 解析 client jar 地址 ----
            string clientUrl;
            using (var doc = JsonDocument.Parse(versionJson))
            {
                if (!doc.RootElement.TryGetProperty("downloads", out var downloads) ||
                    !downloads.TryGetProperty("client", out var client) ||
                    !client.TryGetProperty("url", out var urlEl))
                {
                    throw new InvalidOperationException("版本 JSON 中缺少 client 下载地址。");
                }
                clientUrl = urlEl.GetString();
            }

            // ---- 阶段 2：client jar（占 10~100%）----
            report(10, $"下载 {entry.Id}.jar ...");
            string jarPath = Path.Combine(versionDir, entry.Id + ".jar");

            using (var response = await _http.GetAsync(
                clientUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                long? total = response.Content.Headers.ContentLength;

                using var fs = new FileStream(
                    jarPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
                using var stream = await response.Content.ReadAsStreamAsync();

                var buffer = new byte[81920];
                long readTotal = 0;
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, read);
                    readTotal += read;

                    if (total.HasValue && total.Value > 0)
                    {
                        // 映射到 10~100
                        int percent = 10 + (int)(readTotal * 90 / total.Value);
                        report(percent, $"{entry.Id}.jar 下载中 {percent}%");
                    }
                }
            }

            report(100, $"{entry.Id} 下载完成");
        }

       

        private async Task InstallLoaderAsync(
    string loaderName, string mcVersion, string loaderVersion,
    string mcDir, Action<ProgressBarStyle, int, string> updateUi)
{
    switch (loaderName)
    {
        case "Forge":
            await InstallForgeAsync(mcVersion, loaderVersion, mcDir, updateUi);
            break;
        case "NeoForge":
            await InstallNeoForgeAsync(loaderVersion, mcDir, updateUi);
            break;
        case "Fabric":
            await InstallFabricAsync(mcVersion, loaderVersion, mcDir, updateUi);
            break;
        case "Optifine":
            await InstallOptifineAsync(mcVersion, loaderVersion, mcDir, updateUi);
            break;
        default:
            throw new ArgumentException($"未知加载器：{loaderName}");
    }
}

        private async Task InstallForgeAsync(
    string mcVersion, string forgeVersion, string mcDir,
    Action<ProgressBarStyle, int, string> updateUi)
        {
            // 组合完整版本号，例如：26.2-52.0.1
            string fullVersion = forgeVersion;

            string installerName = $"forge-{fullVersion}-installer.jar";

            // 👇 提供多个下载源：BMCLAPI 镜像优先，官方源兜底
            var downloadUrls = new[]
            {
        // 1. BMCLAPI 镜像（国内极速）
        $"https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{fullVersion}/{installerName}",
        // 2. ForgeCDN（你之前验证过能通的官方 CDN）
        $"https://maven.minecraftforge.net/net/minecraftforge/forge/{fullVersion}/{installerName}",
        // 3. 官方 Maven 源
        $"https://files.minecraftforge.net/maven/net/minecraftforge/forge/{fullVersion}/{installerName}"
    };

            updateUi(ProgressBarStyle.Blocks, 0, $"准备下载 Forge {fullVersion} 安装包...");

            string installerPath = Path.Combine(Path.GetTempPath(), installerName);

            // 使用多源回退下载
            await DownloadFileWithFallbackAsync(downloadUrls, installerPath, percent =>
            {
                updateUi(ProgressBarStyle.Blocks, percent, $"下载 Forge installer {percent}%");
            });

            updateUi(ProgressBarStyle.Marquee, 0, "正在运行 Forge 安装器...");

            // 注意：新版 Forge 如果依然报错，可以尝试把 "--installClient" 改成 "--install-client"
            await RunJavaInstallerAsync(installerPath, $"--installClient \"{mcDir}\"");

            File.Delete(installerPath);
            updateUi(ProgressBarStyle.Blocks, 100, "Forge 安装完成");
        }

        private async Task InstallNeoForgeAsync(
    string neoVersion, string mcDir,
    Action<ProgressBarStyle, int, string> updateUi)
        {
            updateUi(ProgressBarStyle.Blocks, 0, "准备 NeoForge 安装包...");

            string installerName = $"neoforge-{neoVersion}-installer.jar";
            string installerUrl = $"https://maven.neoforged.net/releases/net/neoforged/neoforge/" +
                                  $"{neoVersion}/{installerName}";
            string installerPath = Path.Combine(Path.GetTempPath(), installerName);

            int lastPercent = -1;
            await DownloadFileAsync(installerUrl, installerPath, percent =>
            {
                if (percent == lastPercent) return;
                lastPercent = percent;
                updateUi(ProgressBarStyle.Blocks, percent, $"下载 NeoForge installer {percent}%");
            });

            updateUi(ProgressBarStyle.Marquee, 0, "正在运行 NeoForge 安装器...");
            await RunJavaInstallerAsync(installerPath, $"--installClient \"{mcDir}\"");

            File.Delete(installerPath);
            updateUi(ProgressBarStyle.Blocks, 100, "NeoForge 安装完成");
        }

        private async Task InstallFabricAsync(
    string mcVersion, string loaderVersion, string mcDir,
    Action<ProgressBarStyle, int, string> updateUi)
        {
            updateUi(ProgressBarStyle.Blocks, 0, "准备 Fabric 安装包...");

            string installerUrl = "https://maven.fabricmc.net/net/fabricmc/fabric-installer/" +
                                  "0.11.2/fabric-installer-0.11.2.jar";
            string installerPath = Path.Combine(Path.GetTempPath(), "fabric-installer.jar");

            int lastPercent = -1;
            await DownloadFileAsync(installerUrl, installerPath, percent =>
            {
                if (percent == lastPercent) return;
                lastPercent = percent;
                updateUi(ProgressBarStyle.Blocks, percent, $"下载 Fabric installer {percent}%");
            });

            updateUi(ProgressBarStyle.Marquee, 0, "正在运行 Fabric 安装器...");
            string args = $"client -dir \"{mcDir}\" -mcversion {mcVersion} " +
                          $"-loader {loaderVersion} -noprofile";
            await RunJavaInstallerAsync(installerPath, args);

            File.Delete(installerPath);
            updateUi(ProgressBarStyle.Blocks, 100, "Fabric 安装完成");
        }




        private async Task InstallOptifineAsync(
            string mcVersion, string optifineVersion, string mcDir,
            Action<ProgressBarStyle, int, string> updateUi)
        {
            updateUi(ProgressBarStyle.Marquee, 0, "正在安装 OptiFine，这可能需要几分钟...");

            try
            {
                // 💡 新增：提前清理可能存在的旧文件
                // 假设库下载的文件名格式就是 optifineVersion + ".jar" (或者它内部自己拼的)
                // 我们可以尝试清理整个 Temp 目录下以此命名的文件
                string[] possibleTempFiles = Directory.GetFiles(Path.GetTempPath(), "*OptiFine*.jar");
                foreach (var file in possibleTempFiles)
                {
                    try
                    {
                        // 使用 FileShare.None 尝试独占打开，如果失败说明被占用
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fs.Close();
                        }
                        File.Delete(file);
                        System.Diagnostics.Debug.WriteLine($"[Optifine] 成功清理旧临时文件: {file}");
                    }
                    catch (IOException)
                    {
                        // 如果文件被占用（比如杀毒软件），暂时忽略，留给库自己处理（或者弹窗提示）
                        System.Diagnostics.Debug.WriteLine($"[Optifine] 文件被占用，无法清理: {file}");
                    }
                    catch { /* 忽略其他清理错误 */ }
                }

                var installer = new OptifineInstaller(new HttpClient());
                var allVersions = await installer.GetOptifineVersionsAsync();

                var selected = allVersions.FirstOrDefault(v =>
                    v.Version.Equals(optifineVersion, StringComparison.OrdinalIgnoreCase));

                if (selected == null)
                {
                    // ... (之前的模糊匹配逻辑) ...
                }

                if (selected == null)
                    throw new Exception($"在 OptiFine 库中未找到版本: {optifineVersion}");

                System.Diagnostics.Debug.WriteLine($"[Optifine] 准备安装: {selected.Version}");

                // 调用库的安装方法
                await installer.InstallOptifineAsync(mcDir, selected);

                updateUi(ProgressBarStyle.Blocks, 100, "OptiFine 安装完成");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Optifine] 安装失败: {ex}");
                throw new Exception($"OptiFine 安装失败: {ex.Message}");
            }
        }

        private async Task RunJavaInstallerAsync(string jarPath, string arguments)
        {
            string javaExe = FindJavaExe();
            if (javaExe == null)
                throw new InvalidOperationException("未找到可用的 Java，请先安装 Java 运行环境。");

            var psi = new ProcessStartInfo
            {
                FileName = javaExe,
                Arguments = $"-jar \"{jarPath}\" {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                throw new Exception("无法启动 Java 安装器进程。");

            // 同时开始读取，避免缓冲区满导致死锁
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            // 兼容 .NET Framework / .NET 5+
#if NET5_0_OR_GREATER
            await process.WaitForExitAsync();
#else
    await Task.Run(() => process.WaitForExit());
#endif

            string stdout = await stdoutTask;
            string stderr = await stderrTask;

            if (process.ExitCode != 0)
                throw new Exception($"安装器退出码 {process.ExitCode}：{stderr}");
        }

        private string FindJavaExe()
    {
        // 简单探测：JAVA_HOME → PATH
        string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(javaHome))
        {
            string path = Path.Combine(javaHome, "bin", "java.exe");
            if (File.Exists(path)) return path;
        }

        string pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(pathEnv))
        {
            foreach (var dir in pathEnv.Split(Path.PathSeparator))
            {
                string candidate = Path.Combine(dir, "java.exe");
                if (File.Exists(candidate)) return candidate;
            }
        }

        return null;
    }

        private async Task DownloadFileAsync(string url, string savePath, Action<int> progress = null)
        {
            using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            long? total = response.Content.Headers.ContentLength;
            using var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
            using var stream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[81920];
            long readTotal = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fs.WriteAsync(buffer, 0, read);
                readTotal += read;

                if (total.HasValue && total.Value > 0 && progress != null)
                {
                    int percent = (int)(readTotal * 100 / total.Value);
                    progress(percent);
                }
            }

            progress?.Invoke(100);   // 确保最终 100
        }

        private void 历史记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定清除所有历史记录？", "确认",
        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _db.Clear();   // 清空所有记录，文件保留，变成 []

            // 刷新所有相关控件
            输出文件夹.DataSource = null;
            输出文件夹.Items.Clear();
        }

        private void 缓存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("暂无缓存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private CancellationTokenSource _loaderCheckCts;
        private Dictionary<string, List<string>> _loaderVersionsCache;

        private async void GameVers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _loaderCheckCts?.Cancel();
            _loaderCheckCts = new CancellationTokenSource();
            var token = _loaderCheckCts.Token;

            ResetLoaderRadios();
            加载器版本.DataSource = null;
            加载器版本.Items.Clear();

            if (GameVers.SelectedItem == null ||
                GameVers.SelectedItem.ToString() == "未找到匹配版本")
                return;

            string mcVersion = GameVers.SelectedItem.ToString();

            安装进度.Visible = true;
            安装进度.Style = ProgressBarStyle.Marquee;
            安装进度.MarqueeAnimationSpeed = 30;
            Cursor = Cursors.WaitCursor;

            try
            {
                var all = await LoaderVersionChecker.GetAllAsync(mcVersion);
                if (token.IsCancellationRequested) return;

                _loaderVersionsCache = all;

                // 根据版本列表决定 RadioButton 启用/禁用
                Forge.Enabled = all["Forge"].Count > 0;
                NeoForge.Enabled = all["NeoForge"].Count > 0;
                Fabric.Enabled = all["Fabric"].Count > 0;
                Optifine.Enabled = all["Optifine"].Count > 0;

                // 默认选中“无加载器”
                Forge.Checked = false;
                Fabric.Checked = false;
                NeoForge.Checked = false;
                Optifine.Checked = false;
                无加载器.Checked = true;

                // 填充当前加载器的版本列表
                FillLoaderVersions();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载器查询失败：{ex.Message}");
                无加载器.Checked = true;
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    安装进度.Style = ProgressBarStyle.Blocks;
                    安装进度.Value = 0;
                    //安装进度.Visible = false;
                    Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>根据当前选中的加载器 RadioButton，填充“加载器版本”下拉框</summary>
        private void FillLoaderVersions()
        {
            加载器版本.DataSource = null;
            加载器版本.Items.Clear();

            if (_loaderVersionsCache == null) return;

            string key = 无加载器.Checked ? null :
                         Forge.Checked ? "Forge" :
                         NeoForge.Checked ? "NeoForge" :
                         Fabric.Checked ? "Fabric" :
                         Optifine.Checked ? "Optifine" : null;

            if (key == null || !_loaderVersionsCache.TryGetValue(key, out var versions) || versions.Count == 0)
            {
                加载器版本.Items.Add("无可用版本");
                加载器版本.DropDownStyle = ComboBoxStyle.DropDownList;
                return;
            }

            foreach (var v in versions)
                加载器版本.Items.Add(v);

            加载器版本.DropDownStyle = ComboBoxStyle.DropDownList;
            加载器版本.SelectedIndex = 0;   // 默认选最新
        }

        private void ResetLoaderRadios()
        {
            Forge.Enabled = false;
            NeoForge.Enabled = false;
            Fabric.Enabled = false;
            Optifine.Enabled = false;

            Forge.Checked = false;
            NeoForge.Checked = false;
            Fabric.Checked = false;
            Optifine.Checked = false;

            无加载器.Checked = true;   // 默认选中无加载器
            无加载器.Enabled = true;
        }

        private void Forge_CheckedChanged(object sender, EventArgs e)
        {
            
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;   // 只处理“被选中”那次
            FillLoaderVersions();
        }

        private void Optifine_CheckedChanged(object sender, EventArgs e)
        {

            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;   // 只处理“被选中”那次
            FillLoaderVersions();

        }

        private void Fabric_CheckedChanged(object sender, EventArgs e)
        {

            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;   // 只处理“被选中”那次
            FillLoaderVersions();

        }

        private void NeoForge_CheckedChanged(object sender, EventArgs e)
        {

            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;   // 只处理“被选中”那次
            FillLoaderVersions();

        }

        private void 无加载器_CheckedChanged(object sender, EventArgs e)
        {
            
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;   // 只处理“被选中”那次
            FillLoaderVersions();
        
    }
        

    }
}

