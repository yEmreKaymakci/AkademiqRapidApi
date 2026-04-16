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

            if (!sessions.Any())
            {
                var seedSession = new ChatSession { Title = "Nova ile İlk Sistem Kontrolü", CreatedAt = System.DateTime.UtcNow.AddHours(-2) };
                seedSession.Messages = new List<ChatMessage> {
                    new ChatMessage { Content = "Sistem sağlığı nedir?", Sender = "User", SentAt = System.DateTime.UtcNow.AddHours(-2) },
                    new ChatMessage { Content = "Tüm sistemler sorunsuz çalışıyor. API gecikmeleri normal düzeyde.", Sender = "Nova", SentAt = System.DateTime.UtcNow.AddHours(-2).AddSeconds(5) }
                };
                await _archiveService.CreateSessionAsync(seedSession);
                sessions = await _archiveService.GetAllSessionsAsync();
            }

            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _archiveService.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            return View(session);
        }
    }
}
