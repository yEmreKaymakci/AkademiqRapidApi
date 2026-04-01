using Microsoft.AspNetCore.Mvc;
using AkademiqRapidApi.Services.Interfaces;

namespace AkademiqRapidApi.Controllers
{
    public class CoinController : Controller
    {
        private readonly ICoinService _coinService;

        public CoinController(ICoinService coinService)
        {
            _coinService = coinService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _coinService.GetAllCoinsAsync();
            return View(data);
        }
    }
}