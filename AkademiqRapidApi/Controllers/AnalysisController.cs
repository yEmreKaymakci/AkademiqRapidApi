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

            if (!records.Any())
            {
                var seed = new AnalysisRecord 
                { 
                    Type = "Sistem Taraması", 
                    Summary = "Tüm API servisleri sorunsuz çalışıyor.", 
                    Details = "Google Gemini, Weather, Gas, News aktif. \n\n## Detaylı Rapor\n- Herhangi bir performans darğazı tespit edilmedi.\n- APi gecikmeleri normal seviyelerinde.\n- Kripto servislerinde %0.02 hata oranı (önemsiz).", 
                    CreatedAt = System.DateTime.UtcNow 
                };
                await _analysisService.AddAnalysisAsync(seed);
                records = await _analysisService.GetRecentAnalysesAsync();
            }

            return View(records);
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _analysisService.GetAnalysisByIdAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }
    }
}
