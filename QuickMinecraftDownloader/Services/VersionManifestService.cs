using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using QuickMinecraftDownloader.Models;

namespace QuickMinecraftDownloader.Services
{
    public static class VersionManifestService
    {
        private const string Url = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

        // HttpClient 复用，避免 Socket 耗尽
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// 下载并解析官方版本清单。
        /// </summary>
        public static async Task<VersionManifest> GetAsync()
        {
            string json = await _http.GetStringAsync(Url);

            var manifest = JsonSerializer.Deserialize<VersionManifest>(json, _jsonOpts);
            if (manifest?.Versions == null)
                throw new JsonException("版本清单格式异常或为空");

            return manifest;
        }
    }
}