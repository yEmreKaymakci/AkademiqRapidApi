using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class CoinService : ICoinService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "CryptoCoinsCache";

        public CoinService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _memoryCache = memoryCache;

            var apiHost = _configuration["RapidApi:CoinHost"];
            _httpClient.BaseAddress = new Uri($"https://{apiHost}/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", _configuration["RapidApi:ApiKey"]);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", apiHost);
        }

        public async Task<CoinViewModel.Rootobject> GetAllCoinsAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out CoinViewModel.Rootobject? cachedData) && cachedData != null)
                return cachedData;

            try
            {
                var body = await _httpClient.GetStringAsync("api/tickers/?start=0&limit=100");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<CoinViewModel.Rootobject>(body, options);

                if (data != null) _memoryCache.Set(CacheKey, data, TimeSpan.FromMinutes(30));
                return data ?? new CoinViewModel.Rootobject();
            }
            catch { return new CoinViewModel.Rootobject(); }
        }

        public async Task<List<CoinViewModel.Coin>> GetFeaturedCoinsAsync()
        {
            var allData = await GetAllCoinsAsync();
            if (allData?.data == null) return new List<CoinViewModel.Coin>();

            var targets = new List<string> { "Bitcoin", "Ethereum", "Solana", "XRP" };
            return allData.data.Where(c => targets.Contains(c.name)).ToList();
        }
    }
}
