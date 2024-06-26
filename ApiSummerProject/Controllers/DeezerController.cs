﻿using Microsoft.AspNetCore.Mvc;
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
        #region get artist

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

        [HttpGet("artist/name/{name}")]
        public async Task<IActionResult> GetArtistByName(string name)
        {
            _logger.LogInformation($"Getting artist with name {name}");

            var artist = _artistManager.GetArtistByName(name);
            if (artist != null)
            {
                _logger.LogInformation($"Artist found in cache: {artist.Name}");
                return Ok(artist);
            }

            artist = await _deezerService.SearchArtistByNameAsync(name);
            if (artist == null)
            {
                _logger.LogWarning($"Artist with name {name} not found in Deezer");
                return NotFound("Artist not found");
            }

            var albums = await _deezerService.GetAlbumsByArtistAsync(artist.DeezerId);
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

            _artistManager.Add(artist);
            _logger.LogInformation($"Artist {artist.Name} added to cache");

            return Ok(artist);
        }
        #endregion

        [HttpGet("album/name/{name}")]
        public async Task<IActionResult> GetAlbumByName(string name)
        {
            _logger.LogInformation($"Getting album with name {name}");

            var album = await _deezerService.SearchAlbumByNameAsync(name);
            if (album == null)
            {
                _logger.LogWarning($"Album with name {name} not found in Deezer");
                return NotFound("Album not found");
            }

            return Ok(album);
        }


        [HttpGet("track/name/{name}")]
        public async Task<IActionResult> GetTrackByName(string name)
        {
            _logger.LogInformation($"Getting track with name {name}");

            var tracks = await _deezerService.SearchTracksByNameAsync(name);
            if (tracks == null || tracks.Count == 0)
            {
                _logger.LogWarning($"Track with name {name} not found in Deezer");
                return NotFound("Track not found");
            }

            return Ok(tracks);
        }



    }
}
