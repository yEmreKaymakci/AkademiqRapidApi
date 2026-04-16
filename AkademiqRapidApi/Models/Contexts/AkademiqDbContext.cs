using AkademiqRapidApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AkademiqRapidApi.Models.Contexts
{
    public class AkademiqDbContext : DbContext
    {
        public AkademiqDbContext(DbContextOptions<AkademiqDbContext> options) : base(options)
        {
        }

        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AnalysisRecord> AnalysisRecords { get; set; }
    }
}
