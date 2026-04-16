using System;
using System.Collections.Generic;

namespace AkademiqRapidApi.Models.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Yeni Sohbet";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
