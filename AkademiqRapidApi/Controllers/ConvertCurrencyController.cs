using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
namespace AkademiqRapidApi.Controllers
{
    public class ConvertCurrencyController : Controller
    {
        private readonly IConvertCurrencyService _currencyService;

        public ConvertCurrencyController(IConvertCurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var rates = await _currencyService.GetAllRatesAsync();
            return View(rates);
        }
    }
}
