using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index(string category = "general")
        {
            var news = await _newsService.GetNewsAsync(category);
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsByCategory(string category)
        {
            var news = await _newsService.GetNewsAsync(category);
            return Json(news);
        }
    }
}
