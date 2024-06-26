using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Services
{
    public class ArtistManager
    {
        public List<Artist> Artists { get; private set; } = new List<Artist>();

        public Artist GetArtist(long deezerId)
        {
            return Artists.FirstOrDefault(a => a.DeezerId == deezerId);
        }

        public void Add(Artist artist)
        {
            if (artist == null)
                throw new ArgumentNullException(nameof(artist), "Artist cannot be null");

            if (!Artists.Any(a => a.DeezerId == artist.DeezerId))
            {
                Artists.Add(artist);
            }
        }
    }
}
