using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AkademiqRapidApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MovieService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public MovieService(HttpClient httpClient, IMemoryCache memoryCache, IConfiguration configuration, ILogger<MovieService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;

            var apiKey = _configuration["Tmdb:BearerToken"];

            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}");
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<List<MovieViewModel.Result>> GetMoviesAsync(string category = "popular")
        {
            string cacheKey = $"MovieCache_{category}";
            if (_memoryCache.TryGetValue(cacheKey, out List<MovieViewModel.Result>? cached) && cached != null)
                return cached;

            _logger.LogInformation("MovieService: Fetching movies for category '{cat}'", category);

            // API Endpoint Map
            string endpoint = category.ToLower() switch
            {
                "trending" => "trending/movie/day?language=tr-TR",
                "favorites" => "account/23000330/favorite/movies?language=tr-TR&page=1&sort_by=created_at.desc", // Kullanıcının favori endpointi
                "top_rated" => "movie/top_rated?language=tr-TR&page=1",
                "upcoming" => "movie/upcoming?language=tr-TR&page=1",
                "now_playing" => "movie/now_playing?language=tr-TR&page=1",
                _ => "movie/popular?language=tr-TR&page=1"
            };

            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("MovieService: API çağrısı başarısız. HTTP {status} – Body: {body}", response.StatusCode, body.Length > 200 ? body[..200] : body);
                    return new List<MovieViewModel.Result>();
                }

                var data = JsonSerializer.Deserialize<MovieViewModel.Rootobject>(body, _jsonOpts);

                var validMovies = data?.results?
                    .Where(m => !string.IsNullOrEmpty(m.title) && !string.IsNullOrEmpty(m.poster_path))
                    .ToList() ?? new List<MovieViewModel.Result>();

                _logger.LogInformation("MovieService: {count} geçerli film ayrıştırıldı.", validMovies.Count);

                if (validMovies.Any())
                {
                    _memoryCache.Set(cacheKey, validMovies, TimeSpan.FromMinutes(60));
                }

                return validMovies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MovieService: Filmler alınırken hata oluştu.");
                return new List<MovieViewModel.Result>();
            }
        }
    }
}
