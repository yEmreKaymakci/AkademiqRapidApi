using System;

namespace AkademiqRapidApi.Models.Entities
{
    public class AnalysisRecord
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Genel Kullanım", "API Verileri" vd.
        public string Summary { get; set; }
        public string Details { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
