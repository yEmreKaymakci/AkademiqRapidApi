using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AkademiqRapidApi.Services
{
    /// <summary>
    /// football-data.org ücretsiz API servisi.
    /// Kayıt: https://www.football-data.org/client/register
    /// Ücretsiz plan: dakikada 10 istek, günde sınırsız.
    /// Config key: FootballDataOrg:ApiKey
    /// </summary>
    public class FootballService : IFootballService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FootballService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // football-data.org ücretsiz lig kodları
        // Süper Lig ücretsiz pakette yok — büyük Avrupa ligleri var
        public static readonly Dictionary<int, string> SupportedLeagues = new()
        {
            { 2021, "Premier League" },
            { 2014, "La Liga" },
            { 2002, "Bundesliga" },
            { 2019, "Serie A" },
            { 2015, "Ligue 1" },
            { 2001, "Champions League" },
            { 2003, "Eredivisie" },
            { 2017, "Primeira Liga" }
        };

        public FootballService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, ILogger<FootballService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;

            var apiKey = _configuration["FootballDataOrg:ApiKey"] ?? "";
            _httpClient.BaseAddress = new Uri("https://api.football-data.org/v4/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Auth-Token", apiKey);
            _httpClient.Timeout = TimeSpan.FromSeconds(15);

            if (string.IsNullOrEmpty(apiKey))
                _logger.LogWarning("FootballService: FootballDataOrg:ApiKey bulunamadı! https://www.football-data.org/client/register adresinden ücretsiz alabilirsin.");
        }

        // ── football-data.org → FootballViewModel.Response dönüşümü ──────────
        private static FootballViewModel.Response MapMatch(FdoMatch m)
        {
            // Status eşleştirmesi
            var statusShort = (m.Status ?? "TBD") switch
            {
                "SCHEDULED" or "TIMED" => "NS",
                "IN_PLAY"    => "1H",
                "PAUSED"     => "HT",
                "FINISHED"   => "FT",
                "POSTPONED"  => "PST",
                "CANCELLED"  => "CANC",
                "SUSPENDED"  => "SUSP",
                _            => m.Status ?? "TBD"
            };

            var homeWinner = m.Score?.Winner == "HOME_TEAM" ? true  : (bool?)null;
            var awayWinner = m.Score?.Winner == "AWAY_TEAM" ? true  : (bool?)null;

            return new FootballViewModel.Response
            {
                Fixture = new FootballViewModel.Fixture
                {
                    Id     = m.Id,
                    Date   = m.UtcDate,
                    Status = new FootballViewModel.Status
                    {
                        Short   = statusShort,
                        Long    = m.Status,
                        Elapsed = m.Minute
                    },
                    Venue = m.Venue != null
                        ? new FootballViewModel.Venue { Name = m.Venue.Name, City = m.Venue.City }
                        : null
                },
                League = new FootballViewModel.League
                {
                    Id      = m.Competition?.Id ?? 0,
                    Name    = m.Competition?.Name ?? "",
                    Country = m.Area?.Name ?? "",
                    Logo    = m.Competition?.Emblem ?? ""
                },
                Teams = new FootballViewModel.Teams
                {
                    Home = new FootballViewModel.Team
                    {
                        Id     = m.HomeTeam?.Id ?? 0,
                        Name   = m.HomeTeam?.Name ?? m.HomeTeam?.ShortName ?? "TBD",
                        Logo   = m.HomeTeam?.Crest ?? "",
                        Winner = homeWinner
                    },
                    Away = new FootballViewModel.Team
                    {
                        Id     = m.AwayTeam?.Id ?? 0,
                        Name   = m.AwayTeam?.Name ?? m.AwayTeam?.ShortName ?? "TBD",
                        Logo   = m.AwayTeam?.Crest ?? "",
                        Winner = awayWinner
                    }
                },
                Goals = new FootballViewModel.Goals
                {
                    Home = m.Score?.FullTime?.Home,
                    Away = m.Score?.FullTime?.Away
                },
                Score = new FootballViewModel.Score
                {
                    Fulltime = new FootballViewModel.Goals { Home = m.Score?.FullTime?.Home, Away = m.Score?.FullTime?.Away },
                    Halftime = new FootballViewModel.Goals { Home = m.Score?.HalfTime?.Home, Away = m.Score?.HalfTime?.Away }
                }
            };
        }

        // ── Ham API çağrısı ──────────────────────────────────────────────────
        private async Task<List<FootballViewModel.Response>> FetchAsync(
            string relativeUrl, string cacheKey, TimeSpan cacheDuration)
        {
            if (_cache.TryGetValue(cacheKey, out List<FootballViewModel.Response>? cached) && cached != null)
                return cached;

            _logger.LogInformation("FootballService: GET {url}", relativeUrl);
            try
            {
                var resp = await _httpClient.GetAsync(relativeUrl);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("FootballService: {status} – {body}", resp.StatusCode,
                        body.Length > 300 ? body[..300] : body);
                    return new();
                }

                var root = JsonSerializer.Deserialize<FdoRoot>(body, _jsonOpts);
                var result = (root?.Matches ?? new())
                             .Select(MapMatch)
                             .ToList();

                _logger.LogInformation("FootballService: {count} maç parse edildi.", result.Count);

                if (result.Count > 0)
                    _cache.Set(cacheKey, result, cacheDuration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FootballService: Exception – {url}", relativeUrl);
                return new();
            }
        }

        // ── Canlı Maçlar ─────────────────────────────────────────────────────
        public Task<List<FootballViewModel.Response>> GetLiveFixturesAsync()
            => FetchAsync("matches?status=IN_PLAY,PAUSED", "fdo_live", TimeSpan.FromMinutes(1));

        // ── Bugünün Maçları ───────────────────────────────────────────────────
        public Task<List<FootballViewModel.Response>> GetTodayFixturesAsync()
        {
            string today;
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).ToString("yyyy-MM-dd");
            }
            catch
            {
                today = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd");
            }
            return FetchAsync($"matches?date={today}", $"fdo_today_{today}", TimeSpan.FromMinutes(5));
        }

        // ── Lig Fikstürü (son 7 + önümüzdeki 7 gün) ─────────────────────────
        public Task<List<FootballViewModel.Response>> GetFixturesByLeagueAsync(int leagueId, int season = 2024)
        {
            var from = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            var to   = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");
            return FetchAsync(
                $"competitions/{leagueId}/matches?dateFrom={from}&dateTo={to}",
                $"fdo_league_{leagueId}",
                TimeSpan.FromMinutes(15));
        }

        // ── Dashboard için öne çıkan maç ─────────────────────────────────────
        public async Task<FootballViewModel.Response?> GetFeaturedMatchAsync()
        {
            const string key = "fdo_featured";
            if (_cache.TryGetValue(key, out FootballViewModel.Response? cached)) return cached;

            var live = await GetLiveFixturesAsync();
            var featured = live.FirstOrDefault();

            if (featured == null)
            {
                var today = await GetTodayFixturesAsync();
                featured = today.FirstOrDefault(f => f.Fixture?.Status?.Short == "FT")
                         ?? today.FirstOrDefault();
            }

            if (featured != null)
                _cache.Set(key, featured, TimeSpan.FromMinutes(2));

            return featured;
        }

        // ── football-data.org DTO sınıfları (sadece servis içinde kullanılır) ─
        private record FdoRoot(
            [property: JsonPropertyName("matches")] List<FdoMatch>? Matches
        );

        private record FdoMatch(
            [property: JsonPropertyName("id")]       int Id,
            [property: JsonPropertyName("utcDate")]  string? UtcDate,
            [property: JsonPropertyName("status")]   string? Status,
            [property: JsonPropertyName("minute")]   int? Minute,
            [property: JsonPropertyName("competition")] FdoCompetition? Competition,
            [property: JsonPropertyName("area")]     FdoArea? Area,
            [property: JsonPropertyName("homeTeam")] FdoTeam? HomeTeam,
            [property: JsonPropertyName("awayTeam")] FdoTeam? AwayTeam,
            [property: JsonPropertyName("score")]    FdoScore? Score,
            [property: JsonPropertyName("venue")]    FdoVenue? Venue
        );

        private record FdoCompetition(
            [property: JsonPropertyName("id")]     int Id,
            [property: JsonPropertyName("name")]   string? Name,
            [property: JsonPropertyName("emblem")] string? Emblem
        );

        private record FdoArea(
            [property: JsonPropertyName("name")] string? Name,
            [property: JsonPropertyName("flag")] string? Flag
        );

        private record FdoTeam(
            [property: JsonPropertyName("id")]        int Id,
            [property: JsonPropertyName("name")]      string? Name,
            [property: JsonPropertyName("shortName")] string? ShortName,
            [property: JsonPropertyName("crest")]     string? Crest
        );

        private record FdoScore(
            [property: JsonPropertyName("winner")]   string? Winner,
            [property: JsonPropertyName("fullTime")] FdoGoals? FullTime,
            [property: JsonPropertyName("halfTime")] FdoGoals? HalfTime
        );

        private record FdoGoals(
            [property: JsonPropertyName("home")] int? Home,
            [property: JsonPropertyName("away")] int? Away
        );

        private record FdoVenue(
            [property: JsonPropertyName("name")] string? Name,
            [property: JsonPropertyName("city")] string? City
        );
    }
}
