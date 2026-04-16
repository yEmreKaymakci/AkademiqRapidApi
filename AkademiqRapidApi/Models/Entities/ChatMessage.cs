using System;

namespace AkademiqRapidApi.Models.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string Sender { get; set; } // "User" veya "Nova"
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public ChatSession ChatSession { get; set; }
    }
}
