using System;

namespace QuickMinecraftDownloader.Models
{
    public class VersionEntry
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";      // release / snapshot / old_beta / old_alpha
        public string Url { get; set; } = "";
        public string Time { get; set; } = "";
        public string ReleaseTime { get; set; } = "";
    }
}