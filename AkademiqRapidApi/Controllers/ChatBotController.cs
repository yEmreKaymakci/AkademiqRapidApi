using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly IArchiveService _archiveService;

        public ChatBotController(IGeminiService geminiService, IArchiveService archiveService)
        {
            _geminiService = geminiService;
            _archiveService = archiveService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { success = false, message = "Mesaj boş olamaz." });
            }

            try
            {
                string aiResponse = await _geminiService.GetChatResponseAsync(request.Message);
                return Json(new { success = true, reply = aiResponse });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task StreamMessage([FromBody] ChatRequest request)
        {
            // Session Oluştur
            var session = new AkademiqRapidApi.Models.Entities.ChatSession { 
                Title = request.Message.Length > 25 ? request.Message.Substring(0, 25) + "..." : request.Message, 
                CreatedAt = System.DateTime.UtcNow 
            };
            await _archiveService.CreateSessionAsync(session);

            // User Mesajı Kaydet
            await _archiveService.AddMessageAsync(new AkademiqRapidApi.Models.Entities.ChatMessage { 
                ChatSessionId = session.Id, 
                Content = request.Message, 
                Sender = "User", 
                SentAt = System.DateTime.UtcNow 
            });

            var botResponse = new System.Text.StringBuilder();

            await foreach (var chunk in _geminiService.GetChatResponseStreamAsync(request.Message))
            {
                botResponse.Append(chunk);
                await Response.WriteAsync(chunk);
                await Response.Body.FlushAsync();
            }

            // Bot Mesajı Kaydet
            await _archiveService.AddMessageAsync(new AkademiqRapidApi.Models.Entities.ChatMessage { 
                ChatSessionId = session.Id, 
                Content = botResponse.ToString(), 
                Sender = "Nova", 
                SentAt = System.DateTime.UtcNow 
            });
        }
    }
}
