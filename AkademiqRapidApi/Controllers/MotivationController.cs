using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class MotivationController : Controller
    {
        private readonly IMotivationService _motivationService;

        public MotivationController(IMotivationService motivationService)
        {
            _motivationService = motivationService;
        }

        public async Task<IActionResult> Index()
        {
            var quotes = await _motivationService.GetQuotesLibraryAsync();
            return View(quotes);
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardQuote()
        {
            var quote = await _motivationService.GetQuoteOfTheDayAsync();
            return Json(quote);
        }
    }
}
