using AkademiqRapidApi.Models.Entities;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly IArchiveService _archiveService;

        public ArchiveController(IArchiveService archiveService)
        {
            _archiveService = archiveService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _archiveService.GetAllSessionsAsync();

            // Örnek Dummy Veri eğer veritabanı boşsa arayüz dökülmesin
            if (!sessions.Any())
            {
                sessions.Add(new ChatSession { Id = 1, Title = "Nova ile İlk Sistem Kontrolü", CreatedAt = System.DateTime.UtcNow.AddHours(-2) });
                sessions.Add(new ChatSession { Id = 2, Title = "API Entegrasyon Sorgusu", CreatedAt = System.DateTime.UtcNow.AddDays(-1) });
            }

            return View(sessions);
        }
    }
}
