using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class MusicController : Controller
    {
        private readonly IMusicService _musicService;

        public MusicController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        public async Task<IActionResult> Index()
        {
            var tracks = await _musicService.GetTopTracksAsync();
            return View(tracks);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopTracks()
        {
            var tracks = await _musicService.GetTopTracksAsync();
            return Json(tracks);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylist(long id)
        {
            // İleride farklı çalma listeleri / albümler de çekilebilir
            var tracks = await _musicService.GetPlaylistTracksAsync(id);
            return Json(tracks);
        }
    }
}
