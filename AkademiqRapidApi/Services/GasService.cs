using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class GasService : IGasService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string GasCacheKey = "GasPricesCache";

        public GasService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<List<GasViewModel.Result>> GetAllGasAsync()
        {
            // 1. Önce Cache'e bakıyoruz. Veri orada varsa API'ye hiç gitme!
            if (_memoryCache.TryGetValue(GasCacheKey, out List<GasViewModel.Result> cachedData))
            {
                return cachedData;
            }

            // 2. Cache'te yoksa API'ye git
            var apiKey = _configuration["RapidApi:ApiKey"];
            var apiHost = _configuration["RapidApi:GasHost"];

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://{apiHost}/europeanCountries"),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost },
                },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    // Hata kodunu kontrol et ama EnsureSuccessStatusCode gibi patlatma!
                    if (!response.IsSuccessStatusCode)
                    {
                        // Eğer API hata verirse (429 gibi), boş liste dön veya logla
                        return new List<GasViewModel.Result>();
                    }

                    var body = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var rootNode = JsonSerializer.Deserialize<GasViewModel.Rootobject>(body, options);

                    var resultList = rootNode?.result?.ToList() ?? new List<GasViewModel.Result>();

                    // 3. Veriyi başarılı çektiysek 1 saat boyunca Cache'e at
                    if (resultList.Any())
                    {
                        _memoryCache.Set(GasCacheKey, resultList, TimeSpan.FromHours(1));
                    }

                    return resultList;
                }
            }
            catch (Exception)
            {
                // Bağlantı hataları vb. durumlar için koruma kalkanı
                return new List<GasViewModel.Result>();
            }
        }

        public async Task<List<GasViewModel.Result>> GetThreeGasAsync()
        {
            var allGas = await GetAllGasAsync();
            return allGas.Take(3).ToList();
        }
    }
}