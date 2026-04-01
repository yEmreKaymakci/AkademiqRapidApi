using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class CoinService : ICoinService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "CryptoCoinsCache";

        public CoinService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<CoinViewModel.Rootobject> GetAllCoinsAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out CoinViewModel.Rootobject cachedData)) return cachedData;

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://coinranking1.p.rapidapi.com/coins?referenceCurrencyUuid=yhjMzLPhuIDl&timePeriod=24h&tiers%5B0%5D=1&orderBy=marketCap&orderDirection=desc&limit=50&offset=0"),
                Headers = {
                    { "x-rapidapi-key", _configuration["RapidApi:ApiKey"] },
                    { "x-rapidapi-host", "coinranking1.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new CoinViewModel.Rootobject();

                var body = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<CoinViewModel.Rootobject>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data != null) _memoryCache.Set(CacheKey, data, TimeSpan.FromMinutes(30));
                return data;
            }
            catch { return new CoinViewModel.Rootobject(); }
        }

        public async Task<List<CoinViewModel.Coin>> GetFeaturedCoinsAsync()
        {
            var allData = await GetAllCoinsAsync();
            if (allData?.data?.coins == null) return new List<CoinViewModel.Coin>();

            // Dashboard için istenen spesifik coinleri filtrele
            var targets = new List<string> { "Bitcoin", "Ethereum", "Solana", "XRP" };
            return allData.data.coins
                .Where(c => targets.Contains(c.name))
                .ToList();
        }
    }
}
