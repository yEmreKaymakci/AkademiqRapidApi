using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class ConvertCurrencyService : IConvertCurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "CurrencyRatesCache";

        public ConvertCurrencyService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _memoryCache = memoryCache;

            _httpClient.BaseAddress = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", _configuration["RapidApi:ApiKey"]);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com");
        }

        public async Task<ConvertCurrencyViewModel.Rootobject> GetAllRatesAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out ConvertCurrencyViewModel.Rootobject? cachedData) && cachedData != null)
                return cachedData;

            try
            {
                var body = await _httpClient.GetStringAsync("latest?base=TRY");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<ConvertCurrencyViewModel.Rootobject>(body, options);

                if (data != null) _memoryCache.Set(CacheKey, data, TimeSpan.FromMinutes(30));
                return data ?? new ConvertCurrencyViewModel.Rootobject();
            }
            catch { return new ConvertCurrencyViewModel.Rootobject(); }
        }

        public async Task<Dictionary<string, float>> GetMainRatesForDashboardAsync()
        {
            var data = await GetAllRatesAsync();
            if (data?.rates == null) return new Dictionary<string, float>();

            return new Dictionary<string, float> {
                { "USD", 1 / data.rates.USD },
                { "EUR", 1 / data.rates.EUR },
                { "GBP", 1 / data.rates.GBP }
            };
        }
    }
}
