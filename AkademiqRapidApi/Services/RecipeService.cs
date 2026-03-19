using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;

namespace AkademiqRapidApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // AddHttpClient<IRecipeService, RecipeService> sayesinde HttpClient ve Configuration enjekte edilir.
        public RecipeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<RecipeViewModel.Result>> GetAllRecipesAsync()
        {
            // RapidApi Key ve Host ayarlarımızı System Configuration'dan çekiyoruz.
            var apiKey = _configuration["RapidApi:ApiKey"] ?? "eaa8321078msh8aa48935ef5e1d8p1cb46fjsn6493e1885e8e";
            var apiHost = _configuration["RapidApi:RecipeHost"] ?? "tasty.p.rapidapi.com";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://{apiHost}/recipes/list?from=0&size=50&tags=under_30_minutes"), // Gerçek Endpoint buraya girilecek
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost },
                },
            };

            /* NOT: Aşağıdaki // satırlarını kendi RapidAPI json datanızı modele uydurduğunuzda açın.*/
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                //Json deserialize işlemi
                var values = await response.Content.ReadFromJsonAsync<RecipeViewModel.Rootobject>();
                return values?.results?.ToList() ?? new List<RecipeViewModel.Result>(); ;
            }

        }

        public async Task<RecipeViewModel.Result> GetDailyRecipeAsync()
        {
            //Dashboard sayfasında kullanılmak üzere "Günün Yemeği"ni dönüyoruz.

            var allRecipes = await GetAllRecipesAsync();

            if (allRecipes == null || !allRecipes.Any()) return null;

            Random rand = new Random();

            return allRecipes[rand.Next(allRecipes.Count)];

        }
    }
}
