using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp.Models
{
    public class Playlist
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string Url { get; set; } = "";
        public bool IsPlaylists { get; set; }
        public bool IsLoaded { get; set; }
        public List<Song> Songs { get; set; } = new();
    }
}
