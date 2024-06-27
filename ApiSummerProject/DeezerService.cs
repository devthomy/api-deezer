using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Entities;

namespace Services
{
    public class DeezerService
    {
        private readonly HttpClient _httpClient;

        public DeezerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Artist> GetArtistAsync(long id)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/artist/{id}");
            var deezerArtist = JsonConvert.DeserializeObject<DeezerArtistResponse>(response);

            if (deezerArtist == null)
                return null;

            return new Artist
            {
                DeezerId = deezerArtist.Id,
                Name = deezerArtist.Name,
                PictureSmall = deezerArtist.PictureSmall,
                PictureMedium = deezerArtist.PictureMedium,
                PictureBig = deezerArtist.PictureBig,
                PictureXl = deezerArtist.PictureXl,
            };
        }

        public async Task<Artist> SearchArtistByNameAsync(string name)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/search/artist?q={Uri.EscapeDataString(name)}");
            var searchResponse = JsonConvert.DeserializeObject<DeezerSearchArtistResponse>(response);
            var deezerArtist = searchResponse?.Data?.FirstOrDefault();

            if (deezerArtist == null)
                return null;

            return new Artist
            {
                DeezerId = deezerArtist.Id,
                Name = deezerArtist.Name,
                PictureSmall = deezerArtist.PictureSmall,
                PictureMedium = deezerArtist.PictureMedium,
                PictureBig = deezerArtist.PictureBig,
                PictureXl = deezerArtist.PictureXl,
            };
        }

        public class DeezerSearchArtistResponse
        {
            public List<DeezerArtistResponse> Data { get; set; }
        }




        public async Task<List<Album>> GetAlbumsByArtistAsync(long artistId)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/artist/{artistId}/albums");
            var albumsResponse = JsonConvert.DeserializeObject<AlbumsResponse>(response);
            return albumsResponse.Data ?? new List<Album>();
        }

        public async Task<List<Track>> GetTracksByAlbumAsync(long albumId)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/album/{albumId}/tracks");
            var tracksResponse = JsonConvert.DeserializeObject<TracksResponse>(response);
            return tracksResponse.Data ?? new List<Track>();
        }

        public async Task<Album> SearchAlbumByNameAsync(string name)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/search/album?q={Uri.EscapeDataString(name)}");
            var searchResponse = JsonConvert.DeserializeObject<DeezerSearchAlbumResponse>(response);
            var deezerAlbum = searchResponse?.Data?.FirstOrDefault();

            if (deezerAlbum == null)
                return null;

            var tracksResponse = await _httpClient.GetStringAsync($"https://api.deezer.com/album/{deezerAlbum.Id}/tracks");
            var tracks = JsonConvert.DeserializeObject<TracksResponse>(tracksResponse)?.Data ?? new List<Track>();

            var artistResponse = await _httpClient.GetStringAsync($"https://api.deezer.com/artist/{deezerAlbum.Artist.Id}");
            var deezerArtist = JsonConvert.DeserializeObject<DeezerArtistResponse>(artistResponse);

            return new Album
            {
                Id = deezerAlbum.Id,
                Title = deezerAlbum.Title,
                Cover = deezerAlbum.Cover,
                Artist = new Artist
                {
                    DeezerId = deezerArtist.Id,
                    Name = deezerArtist.Name,
                    PictureSmall = deezerArtist.PictureSmall,
                    PictureMedium = deezerArtist.PictureMedium,
                    PictureBig = deezerArtist.PictureBig,
                    PictureXl = deezerArtist.PictureXl,
                },
                Tracks = tracks
            };
        }

        public async Task<List<Track>> SearchTracksByNameAsync(string name)
        {
            var response = await _httpClient.GetStringAsync($"https://api.deezer.com/search/track?q={Uri.EscapeDataString(name)}");
            var searchResponse = JsonConvert.DeserializeObject<DeezerSearchTrackResponse>(response);
            return searchResponse?.Data ?? new List<Track>();
        }

        public class DeezerSearchTrackResponse
        {
            public List<Track> Data { get; set; }
        }


        public class DeezerSearchAlbumResponse
        {
            public List<DeezerAlbumResponse> Data { get; set; }
        }

        public class DeezerAlbumResponse
        {
            public long Id { get; set; }
            public string Title { get; set; }
            [JsonProperty("cover")]
            public string Cover { get; set; }
            [JsonProperty("artist")]
            public DeezerArtistResponse Artist { get; set; }
        }


    }

    public class DeezerArtistResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("picture_small")]
        public string PictureSmall { get; set; }
        [JsonProperty("picture_medium")]
        public string PictureMedium { get; set; }
        [JsonProperty("picture_big")]
        public string PictureBig { get; set; }
        [JsonProperty("picture_xl")]
        public string PictureXl { get; set; }
    }

    public class AlbumsResponse
    {
        public List<Album> Data { get; set; }
    }

    public class TracksResponse
    {
        public List<Track> Data { get; set; }
    }
}
