using System.Collections.Generic;

namespace QuickMinecraftDownloader.Models
{
    public class VersionManifest
    {
        public LatestVersions Latest { get; set; }
        public List<VersionEntry> Versions { get; set; } = new List<VersionEntry>();
    }

    public class LatestVersions
    {
        public string Release { get; set; } = "";
        public string Snapshot { get; set; } = "";
    }
}