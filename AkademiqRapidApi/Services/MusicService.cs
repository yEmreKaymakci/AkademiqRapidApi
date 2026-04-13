using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AkademiqRapidApi.Services
{
    public class MusicService : IMusicService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MusicService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public MusicService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<MusicService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://api.deezer.com/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<List<MusicViewModel.Track>> GetTopTracksAsync()
        {
            string cacheKey = "DeezerTopTracks";
            if (_memoryCache.TryGetValue(cacheKey, out List<MusicViewModel.Track>? cached) && cached != null)
                return cached;

            _logger.LogInformation("MusicService: Fetching Global Top Tracks from Deezer Chart");

            try
            {
                // chart/0/tracks -> Global Top Tracks list
                var response = await _httpClient.GetAsync("chart/0/tracks");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("MusicService: API failed {status} - Body: {body}", response.StatusCode, body.Length > 200 ? body[..200] : body);
                    return new List<MusicViewModel.Track>();
                }

                var data = JsonSerializer.Deserialize<MusicViewModel.Rootobject>(body, _jsonOpts);

                var validTracks = data?.data?
                    .Where(t => !string.IsNullOrEmpty(t.title) && !string.IsNullOrEmpty(t.preview))
                    .ToList() ?? new List<MusicViewModel.Track>();

                _logger.LogInformation("MusicService: Received {count} previewable tracks.", validTracks.Count);

                if (validTracks.Any())
                {
                    _memoryCache.Set(cacheKey, validTracks, TimeSpan.FromMinutes(30)); // 30 min cache
                }

                return validTracks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MusicService: Error fetching tracks.");
                return new List<MusicViewModel.Track>();
            }
        }

        public async Task<List<MusicViewModel.Track>> GetPlaylistTracksAsync(long playlistId)
        {
            string cacheKey = $"DeezerPlaylist_{playlistId}";
            if (_memoryCache.TryGetValue(cacheKey, out List<MusicViewModel.Track>? cached) && cached != null)
                return cached;

            try
            {
                var response = await _httpClient.GetAsync($"playlist/{playlistId}/tracks");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) return new List<MusicViewModel.Track>();

                var data = JsonSerializer.Deserialize<MusicViewModel.Rootobject>(body, _jsonOpts);
                var validTracks = data?.data?.Where(t => !string.IsNullOrEmpty(t.preview)).ToList() ?? new List<MusicViewModel.Track>();

                if (validTracks.Any()) _memoryCache.Set(cacheKey, validTracks, TimeSpan.FromMinutes(60));

                return validTracks;
            }
            catch (Exception)
            {
                return new List<MusicViewModel.Track>();
            }
        }
    }
}
