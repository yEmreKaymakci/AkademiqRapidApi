using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class GasService : IGasService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string GasCacheKey = "GasPricesCache";

        public GasService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _memoryCache = memoryCache;

            var apiHost = _configuration["RapidApi:GasHost"] ?? "gas-prices-by-country.p.rapidapi.com";
            _httpClient.BaseAddress = new Uri($"https://{apiHost}/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", _configuration["RapidApi:ApiKey"]);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", apiHost);
        }

        public async Task<List<GasViewModel.Result>> GetAllGasAsync()
        {
            if (_memoryCache.TryGetValue(GasCacheKey, out List<GasViewModel.Result>? cachedData) && cachedData != null)
                return cachedData;

            try
            {
                var body = await _httpClient.GetStringAsync("europeanCountries");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var rootNode = JsonSerializer.Deserialize<GasViewModel.Rootobject>(body, options);
                var resultList = rootNode?.result?.ToList() ?? new List<GasViewModel.Result>();

                if (resultList.Any())
                    _memoryCache.Set(GasCacheKey, resultList, TimeSpan.FromHours(1));

                return resultList;
            }
            catch
            {
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