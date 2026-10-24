using System.IO;
using System.Linq;

namespace QuickMinecraftDownloader.Services
{
    public static class DriveScanner
    {
        public static List<string> GetAllDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady)                                    // 排除未就绪（如空光驱）
                .Where(d => d.DriveType == DriveType.Fixed                  // 本地硬盘
                         || d.DriveType == DriveType.Removable)             // U 盘/移动硬盘
                                                                            // 想扫网络盘就加 DriveType.Network，但会很慢
                .Select(d => d.RootDirectory.FullName)                    // "C:\" "D:\"
                .ToList();
        }
    }
}