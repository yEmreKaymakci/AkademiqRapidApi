using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;

namespace AkademiqRapidApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RecipeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var apiHost = _configuration["RapidApi:RecipeHost"] ?? "tasty.p.rapidapi.com";
            _httpClient.BaseAddress = new Uri($"https://{apiHost}/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", _configuration["RapidApi:ApiKey"]);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", apiHost);
        }

        public async Task<List<RecipeViewModel.Result>> GetAllRecipesAsync()
        {
            try
            {
                using var response = await _httpClient.GetAsync("recipes/list?from=0&size=50&tags=under_30_minutes");
                if (!response.IsSuccessStatusCode) return new List<RecipeViewModel.Result>();

                var values = await response.Content.ReadFromJsonAsync<RecipeViewModel.Rootobject>();
                return values?.results?.ToList() ?? new List<RecipeViewModel.Result>();
            }
            catch { return new List<RecipeViewModel.Result>(); }
        }

        public async Task<RecipeViewModel.Result?> GetDailyRecipeAsync()
        {
            var allRecipes = await GetAllRecipesAsync();
            if (allRecipes == null || !allRecipes.Any()) return null;
            return allRecipes[new Random().Next(allRecipes.Count)];
        }
    }
}
