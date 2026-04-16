using AkademiqRapidApi.Models.Contexts;
using AkademiqRapidApi.Models.Entities;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkademiqRapidApi.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly AkademiqDbContext _context;

        public AnalysisService(AkademiqDbContext context)
        {
            _context = context;
        }

        public async Task AddAnalysisAsync(AnalysisRecord record)
        {
            await _context.AnalysisRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AnalysisRecord>> GetRecentAnalysesAsync()
        {
            return await _context.AnalysisRecords.OrderByDescending(x => x.CreatedAt).Take(20).ToListAsync();
        }

        public async Task<AnalysisRecord> GetAnalysisByIdAsync(int id)
        {
            return await _context.AnalysisRecords.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
