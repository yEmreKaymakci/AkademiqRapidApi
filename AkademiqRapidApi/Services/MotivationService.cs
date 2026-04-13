using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AkademiqRapidApi.Services
{
    public class MotivationService : IMotivationService
    {
        private readonly HttpClient _favqsClient;
        private readonly HttpClient _translateClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MotivationService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public MotivationService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<MotivationService> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;

            // FavQs Client
            _favqsClient = httpClientFactory.CreateClient("FavQs");
            _favqsClient.BaseAddress = new Uri("https://favqs.com/api/");
            var apiKey = _configuration["FavQsApi:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _favqsClient.DefaultRequestHeaders.Add("Authorization", $"Token token=\"{apiKey}\"");
            }

            // Google Translate Public Client
            _translateClient = httpClientFactory.CreateClient("GoogleTranslate");
        }

        public async Task<QuoteViewModel.QuoteItem?> GetQuoteOfTheDayAsync()
        {
            string cacheKey = "FavQs_QoTD";
            if (_memoryCache.TryGetValue(cacheKey, out QuoteViewModel.QuoteItem? cached) && cached != null)
                return cached;

            try
            {
                var response = await _favqsClient.GetAsync("qotd");
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<QuoteViewModel.QotdResponse>(body, _jsonOpts);

                    if (data?.quote != null)
                    {
                        // Çeviri yap
                        data.quote.TranslatedBody = await TranslateTextAsync(data.quote.body);

                        _memoryCache.Set(cacheKey, data.quote, TimeSpan.FromHours(4)); // QOTD her 4 saatte bir güncellensin
                        return data.quote;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MotivationService: QoTD Fetch error.");
            }

            return null;
        }

        public async Task<List<QuoteViewModel.QuoteItem>> GetQuotesLibraryAsync()
        {
            string cacheKey = "FavQs_Library";
            if (_memoryCache.TryGetValue(cacheKey, out List<QuoteViewModel.QuoteItem>? cached) && cached != null)
                return cached;

            try
            {
                // Rastgele quotes getir (FavQs Authorization gerektirir)
                var response = await _favqsClient.GetAsync("quotes");
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<QuoteViewModel.QuoteResponse>(body, _jsonOpts);

                    var validQuotes = data?.quotes?.Where(q => !string.IsNullOrEmpty(q.body)).ToList() ?? new List<QuoteViewModel.QuoteItem>();

                    // Hepsi için çeviri yap (Süreci paralel işletelim ki hızlı dolsun)
                    var translateTasks = validQuotes.Select(async q =>
                    {
                        var tr = await TranslateTextAsync(q.body);
                        q.TranslatedBody = string.IsNullOrEmpty(tr) ? q.body : tr; // Hata olursa orijinal kalsın
                    });

                    await Task.WhenAll(translateTasks);

                    if (validQuotes.Any())
                    {
                        _memoryCache.Set(cacheKey, validQuotes, TimeSpan.FromMinutes(60));
                    }

                    return validQuotes;
                }
                else
                {
                    _logger.LogWarning("MotivationService: FavQs Library returned {status}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MotivationService: Library fetch error.");
            }

            return new List<QuoteViewModel.QuoteItem>();
        }

        private async Task<string> TranslateTextAsync(string? originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText)) return "";

            try
            {
                // Google Translate Free Endpoint URL (JSON Dizi Formatında)
                // client=gtx&sl=en&tl=tr&dt=t&q={text}
                var encodedParams = Uri.EscapeDataString(originalText);
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=tr&dt=t&q={encodedParams}";

                var response = await _translateClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var resultStr = await response.Content.ReadAsStringAsync();
                    
                    // Sonuç json iç içe geçmiş array formatında gelir: [[["Çeviri","Orijinal",null,null,1]],null,"en",...]
                    using var doc = JsonDocument.Parse(resultStr);
                    var rootElement = doc.RootElement;
                    if (rootElement.ValueKind == JsonValueKind.Array && rootElement.GetArrayLength() > 0)
                    {
                        var translations = rootElement[0];
                        if (translations.ValueKind == JsonValueKind.Array && translations.GetArrayLength() > 0)
                        {
                            var sb = new System.Text.StringBuilder();
                            foreach (var segment in translations.EnumerateArray())
                            {
                                if (segment.ValueKind == JsonValueKind.Array && segment.GetArrayLength() > 0)
                                {
                                    sb.Append(segment[0].GetString());
                                }
                            }
                            return sb.ToString(); // Tüm parçacıkları birleştir
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Translate error");
            }

            return originalText; // Hata anında en azından orjinal metni göster
        }
    }
}
