using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IRecipeService
    {
        // Controller'in index sayfasında listelemek için (Örn: 100 tarif)
        Task<List<RecipeViewModel.Result>> GetAllRecipesAsync();

        // Dashboard sayfasında sadece günün yemeği (1 adet veya rastgele) için
        Task<RecipeViewModel.Result> GetDailyRecipeAsync();
    }
}
