using AkademiqRapidApi.Models.Entities;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IAnalysisService
    {
        Task<List<AnalysisRecord>> GetRecentAnalysesAsync();
        Task AddAnalysisAsync(AnalysisRecord record);
        Task<AnalysisRecord> GetAnalysisByIdAsync(int id);
    }
}
