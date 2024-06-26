using System;
using System.Collections.Generic;

namespace Entities
{
    public class Artist
    {
        public long DeezerId { get; set; }
        public string Name { get; set; }
        public string PictureSmall { get; set; }
        public string PictureMedium { get; set; }
        public string PictureBig { get; set; }
        public string PictureXl { get; set; }
        public List<Album> Albums { get; set; } = new List<Album>();
        public string Id { get; set; }

        public Artist()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
