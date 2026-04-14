using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AkademiqRapidApi.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewsService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public NewsService(HttpClient httpClient, IMemoryCache memoryCache, IConfiguration configuration, ILogger<NewsService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;

            // API key'i config'den çekiyoruz
            var apiKey = _configuration["NewsApi:ApiKey"];

            _httpClient.BaseAddress = new Uri("https://newsapi.org/v2/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "AkademiQ-Dashboard");
            // NewsAPI anahtarı authorization header'ı olarak da verebiliriz veya URL'ye query parametri olarak ekleyebiliriz.
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<List<NewsViewModel.Article>> GetNewsAsync(string category = "general")
        {
            string cacheKey = $"NewsCache_{category}";
            if (_memoryCache.TryGetValue(cacheKey, out List<NewsViewModel.Article>? cached) && cached != null)
                return cached;

            _logger.LogInformation("NewsService: Fetching news for category '{cat}'", category);

            // pageSize={count} ile haberi limitleyebiliriz, default 20.
            // NOT: NewsAPI'nin ücretsiz katmanı Türkiye (tr) için sonuçları kapatmış olabilir. Global için "us" kullanıyoruz.
            var url = $"top-headlines?country=us&category={category}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("NewsService: API çağrısı başarısız. HTTP {status} – Body: {body}", response.StatusCode, body.Length > 200 ? body[..200] : body);
                    return new List<NewsViewModel.Article>();
                }

                var data = JsonSerializer.Deserialize<NewsViewModel.Rootobject>(body, _jsonOpts);

                var validArticles = data?.Articles?
                    .Where(a => !string.IsNullOrEmpty(a.UrlToImage) && !string.IsNullOrEmpty(a.Title))
                    .ToList() ?? new List<NewsViewModel.Article>();

                _logger.LogInformation("NewsService: {count} geçerli haber ayrıştırıldı.", validArticles.Count);

                if (validArticles.Any())
                {
                    // Haberler görece daha taze olduğu için 30 dakika iyi
                    _memoryCache.Set(cacheKey, validArticles, TimeSpan.FromMinutes(30));
                }

                return validArticles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NewsService: Haberler alınırken hata oluştu.");
                return new List<NewsViewModel.Article>();
            }
        }
    }
}