namespace QuickMinecraftDownloader.Models
{
    public class LoaderInfo
    {
        public string LoaderName { get; set; } = "";
        public List<string> Versions { get; set; } = new List<string>();
        public bool HasAny => Versions.Count > 0;
    }
}