using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuickMinecraftDownloader.Services
{
    public static class FolderScanner
    {
        /// <summary>
        /// 递归扫描指定根目录，返回名称匹配的文件夹完整路径。
        /// </summary>
        /// <param name="rootPath">起始根目录，如 @"C:\" 或 @"D:\Games"</param>
        /// <param name="keyword">文件夹名包含的关键字，如 "minecraft"（忽略大小写）</param>
        /// <param name="exactMatch">true=名称完全相等；false=包含即可</param>
        /// <param name="maxDepth">最大递归深度，防止无限深入</param>
        /// <param name="maxResults">最多返回多少条，防止结果爆炸</param>
        public static List<string> Scan(
            string rootPath,
            string keyword,
            bool exactMatch = false,
            int maxDepth = 6,
            int maxResults = 200,
            Action<string> onFound = null)
        {
            var results = new List<string>();
            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
                return results;

            var stack = new Stack<(string path, int depth)>();
            stack.Push((rootPath, 0));

            while (stack.Count > 0 && results.Count < maxResults)
            {
                var (current, depth) = stack.Pop();

                // 1. 先判断当前目录名是否匹配
                string name = Path.GetFileName(current);
                if (!string.IsNullOrEmpty(name) && IsMatch(name, keyword, exactMatch))
                {
                    results.Add(current);
                    onFound?.Invoke(current);
                    continue; // 命中的目录不再往下钻，避免把子目录也收进来
                }

                if (depth >= maxDepth) continue;

                // 2. 枚举子目录，权限异常要吞掉
                try
                {
                    foreach (var dir in Directory.EnumerateDirectories(current))
                    {
                        // 跳过符号链接/系统关键目录，可按需调整
                        var attr = File.GetAttributes(dir);
                        if ((attr & FileAttributes.ReparsePoint) != 0) continue;

                        stack.Push((dir, depth + 1));
                    }
                }
                catch (UnauthorizedAccessException) { /* 无权限，跳过 */ }
                catch (IOException) { /* 路径问题，跳过 */ }
                catch (Exception) { /* 其他异常，兜底 */ }
            }

            return results;
        }

        private static bool IsMatch(string folderName, string keyword, bool exactMatch)
        {
            if (exactMatch)
                return string.Equals(folderName, keyword, StringComparison.OrdinalIgnoreCase);

            return folderName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}