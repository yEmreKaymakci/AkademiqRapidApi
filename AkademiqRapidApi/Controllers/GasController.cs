using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AkademiqRapidApi.Controllers
{
    public class GasController : Controller
    {
        private readonly IGasService _gasService;

        public GasController(IGasService gasService)
        {
            _gasService = gasService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _gasService.GetAllGasAsync();

            // Verileri ülke adına göre A'dan Z'ye sıralayarak gönderiyoruz
            var sortedValues = values.OrderBy(x => x.country).ToList();

            return View(sortedValues);
        }
    }
}
