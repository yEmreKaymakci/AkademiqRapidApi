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
                RequestUri = new Uri("https://coinlore-cryptocurrency.p.rapidapi.com/api/tickers/?start=0&limit=100"),
                Headers = {
                    { "x-rapidapi-key", _configuration["RapidApi:ApiKey"] },
                    { "x-rapidapi-host", "coinlore-cryptocurrency.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new CoinViewModel.Rootobject();

                // .NET 8/9 standartlarına uygun performanslı deserialization (System.Net.Http.Json kullanımı)
                var data = await response.Content.ReadFromJsonAsync<CoinViewModel.Rootobject>();

                if (data != null) _memoryCache.Set(CacheKey, data, TimeSpan.FromMinutes(30));
                return data;
            }
            catch { return new CoinViewModel.Rootobject(); }
        }

        public async Task<List<CoinViewModel.Coin>> GetFeaturedCoinsAsync()
        {
            var allData = await GetAllCoinsAsync();
            if (allData?.data == null) return new List<CoinViewModel.Coin>(); // allData.data.coins yerine root object'ten direkt array geliyor (allData.data)

            // Dashboard için istenen spesifik coinleri filtrele
            var targets = new List<string> { "Bitcoin", "Ethereum", "Solana", "XRP" };
            return allData.data
                .Where(c => targets.Contains(c.name))
                .ToList();
        }
    }
}
