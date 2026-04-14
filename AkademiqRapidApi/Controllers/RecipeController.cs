using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        // Controller sadece Service arayüzüne (Interface) bağımlıdır. HTTP veya json gibi yapılarla Controller ilgilenmez.
        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index()
        {
            // Servis üzerinden tarifleri getir (İster 100 ister sahte veri gelsin Controller oraya bakmaz)
            var values = await _recipeService.GetAllRecipesAsync();

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyRecipe()
        {
            var values = await _recipeService.GetAllRecipesAsync();
            return Json(values);
        }
    }
}
