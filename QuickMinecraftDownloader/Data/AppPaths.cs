using System;
using System.IO;

namespace QuickMinecraftDownloader.Data
{
    public static class AppPaths
    {
        public static readonly string RootDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "QuickMinecraftDownloader");

        public static string MemoriesFile => Path.Combine(RootDir, "memories.json");

        public static void EnsureRoot()
        {
            if (!Directory.Exists(RootDir))
                Directory.CreateDirectory(RootDir);
        }
    }
}