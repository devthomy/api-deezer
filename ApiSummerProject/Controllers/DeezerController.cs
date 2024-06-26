using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Entities;
using Services;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeezerController : ControllerBase
    {
        private readonly DeezerService _deezerService;
        private readonly ArtistManager _artistManager;
        private readonly ILogger<DeezerController> _logger;

        private static readonly ConcurrentDictionary<long, object> _artistLocks = new ConcurrentDictionary<long, object>();

        public DeezerController(DeezerService deezerService, ArtistManager artistManager, ILogger<DeezerController> logger)
        {
            _deezerService = deezerService;
            _artistManager = artistManager;
            _logger = logger;
        }

        [HttpGet("artist/{id}")]
        public async Task<IActionResult> GetArtist(long id)
        {
            _logger.LogInformation($"Getting artist with ID {id}");

            var lockObject = _artistLocks.GetOrAdd(id, new object());

            Artist artist;
            lock (lockObject)
            {
                artist = _artistManager.GetArtist(id);
                if (artist != null)
                {
                    _logger.LogInformation($"Artist found in cache: {artist.Name}");
                    return Ok(artist);
                }
            }

            artist = await _deezerService.GetArtistAsync(id);
            if (artist == null)
            {
                _logger.LogWarning($"Artist with ID {id} not found in Deezer");
                return NotFound("Artist not found");
            }

            var albums = await _deezerService.GetAlbumsByArtistAsync(id);
            if (albums != null)
            {
                foreach (var album in albums)
                {
                    var tracks = await _deezerService.GetTracksByAlbumAsync(album.Id);
                    if (tracks != null)
                    {
                        album.Tracks = tracks;
                    }
                }
                artist.Albums = albums;
            }

            lock (lockObject)
            {
                _artistManager.Add(artist);
                _logger.LogInformation($"Artist {artist.Name} added to cache");
            }

            _artistLocks.TryRemove(id, out _);

            return Ok(artist);
        }

    }
}
