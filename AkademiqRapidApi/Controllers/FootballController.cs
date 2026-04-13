using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class FootballController : Controller
    {
        private readonly IFootballService _footballService;

        public FootballController(IFootballService footballService)
        {
            _footballService = footballService;
        }

        // GET /Football
        public async Task<IActionResult> Index()
        {
            // Başlangıç: bugünün maçlarını getir
            var fixtures = await _footballService.GetTodayFixturesAsync();
            return View(fixtures);
        }

        // AJAX: GET /Football/GetLive
        [HttpGet]
        public async Task<IActionResult> GetLive()
        {
            var fixtures = await _footballService.GetLiveFixturesAsync();
            return Json(fixtures);
        }

        // AJAX: GET /Football/GetToday
        [HttpGet]
        public async Task<IActionResult> GetToday()
        {
            var fixtures = await _footballService.GetTodayFixturesAsync();
            return Json(fixtures);
        }

        // AJAX: GET /Football/GetByLeague?leagueId=39&season=2024
        [HttpGet]
        public async Task<IActionResult> GetByLeague(int leagueId, int season = 2024)
        {
            var fixtures = await _footballService.GetFixturesByLeagueAsync(leagueId, season);
            return Json(fixtures);
        }

        // AJAX: GET /Football/GetFeatured — Dashboard widget için
        [HttpGet]
        public async Task<IActionResult> GetFeatured()
        {
            var match = await _footballService.GetFeaturedMatchAsync();
            if (match == null) return Json(null);
            return Json(match);
        }
    }
}
