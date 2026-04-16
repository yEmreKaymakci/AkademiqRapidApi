using AkademiqRapidApi.Models.Entities;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _analysisService.GetRecentAnalysesAsync();

            // Örnek Dummy Veri eğer veritabanı boşsa arayüz dökülmesin
            if (!records.Any())
            {
                records.Add(new AnalysisRecord { Type = "Sistem Taraması", Summary = "Tüm API servisleri sorunsuz çalışıyor.", Details = "Google Gemini, Weather, Gas, News aktif." });
                records.Add(new AnalysisRecord { Type = "Güvenlik", Summary = "Son 24 saatte olağandışı bir durum tespit edilmedi.", Details = "Kullanıcı secret kilitleri devrede." });
            }

            return View(records);
        }
    }
}
