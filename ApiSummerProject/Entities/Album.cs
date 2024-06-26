using System.Collections.Generic;

namespace Entities
{
    public class Album
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public Artist Artist { get; set; }
        public List<Track> Tracks { get; set; } = new List<Track>();
    }

    public class AlbumResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public Artist Artist { get; set; }
        public TracksData Tracks { get; set; }
    }

    public class TracksData
    {
        public List<Track> Data { get; set; } = new List<Track>();
    }
}
