using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IFootballService
    {
        /// <summary>Anlık canlı maçları getirir.</summary>
        Task<List<FootballViewModel.Response>> GetLiveFixturesAsync();

        /// <summary>Belirli bir ligi sezon bazında getirir.</summary>
        Task<List<FootballViewModel.Response>> GetFixturesByLeagueAsync(int leagueId, int season = 2024);

        /// <summary>Bugünün maçlarını getirir.</summary>
        Task<List<FootballViewModel.Response>> GetTodayFixturesAsync();

        /// <summary>Dashboard için en güncel maçı döndürür (canlı varsa canlı, yoksa dünün son maçı).</summary>
        Task<FootballViewModel.Response?> GetFeaturedMatchAsync();
    }
}
