namespace AkademiqRapidApi.Models
{
    public class QuoteViewModel
    {
        public class QuoteResponse
        {
            public int? count { get; set; }
            public List<QuoteItem>? quotes { get; set; }
        }

        public class QotdResponse
        {
            public QuoteItem? quote { get; set; }
        }

        public class QuoteItem
        {
            public int id { get; set; }
            public string? body { get; set; }     // Orijinal İngilizce
            public string? author { get; set; }
            public string? url { get; set; }
            public int? upvotes_count { get; set; }
            public int? downvotes_count { get; set; }
            
            // Sonradan doldurulacak özellikler
            public string? TranslatedBody { get; set; }  // Çevrilmiş Türkçe metin
        }
    }
}
