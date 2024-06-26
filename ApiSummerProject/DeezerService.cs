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
