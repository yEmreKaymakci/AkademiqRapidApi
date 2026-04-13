using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            // Ana sayfa için default listemiz (Örn: Popüler filmler)
            var movies = await _movieService.GetMoviesAsync("popular");
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesByCategory(string category)
        {
            var movies = await _movieService.GetMoviesAsync(category);
            return Json(movies);
        }
    }
}
