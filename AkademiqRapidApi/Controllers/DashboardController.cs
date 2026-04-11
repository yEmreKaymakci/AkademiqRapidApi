using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    // Dashboard sayfası hava durumu verisini WeatherController.GetCurrentLocationWeather()
    // üzerinden AJAX ile yükler — bu controller sadece view döndürür.
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
