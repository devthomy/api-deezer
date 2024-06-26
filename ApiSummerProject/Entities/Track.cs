namespace Entities
{
    public class Track
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public Album Album { get; set; }
        public Artist Artist { get; set; }
        public int Duration { get; set; }
        public int TrackPosition { get; set; }
        public string Preview { get; set; }
    }
}
