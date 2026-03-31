using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class ConvertCurrencyService : IConvertCurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "CurrencyRatesCache";

        public ConvertCurrencyService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<ConvertCurrencyViewModel.Rootobject> GetAllRatesAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out ConvertCurrencyViewModel.Rootobject cachedData)) return cachedData;

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?base=TRY"),
                Headers = {
                    { "x-rapidapi-key", _configuration["RapidApi:ApiKey"] },
                    { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new ConvertCurrencyViewModel.Rootobject();

                var body = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ConvertCurrencyViewModel.Rootobject>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data != null) _memoryCache.Set(CacheKey, data, TimeSpan.FromMinutes(30));
                return data;
            }
            catch { return new ConvertCurrencyViewModel.Rootobject(); }
        }

        public async Task<Dictionary<string, float>> GetMainRatesForDashboardAsync()
        {
            var data = await GetAllRatesAsync();
            if (data?.rates == null) return new Dictionary<string, float>();

            // 1 TL = x USD ise, 1 USD = 1/x TL'dir mantığıyla çeviriyoruz
            return new Dictionary<string, float> {
                { "USD", 1 / data.rates.USD },
                { "EUR", 1 / data.rates.EUR },
                { "GBP", 1 / data.rates.GBP }
            };
        }
    }
}
