using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickMinecraftDownloader.Models
{
    public class MemoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}