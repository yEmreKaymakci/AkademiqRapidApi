using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        // 4 sabit dünya şehri
        private static readonly (string city, string country)[] _featuredCities =
        {
            ("New York",  "US"),
            ("Istanbul",  "TR"),
            ("Berlin",    "DE"),
            ("Paris",     "FR"),
        };

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // ── 1. Ana Sayfa ────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            // IP'den konum + 15 günlük tahmin & 4 şehir kartı paralel
            var (ipTask, citiesTask) = (
                _weatherService.GetWeatherByIpAsync(),
                _weatherService.GetWeatherForCitiesAsync(_featuredCities)
            );
            await Task.WhenAll(ipTask, citiesTask);

            var (location, forecast) = await ipTask;
            var featuredCities = await citiesTask;

            // Türkiye — 1. sayfa (ilk 12 şehir)
            var (turkeyItems, turkeyTotal) = await _weatherService.GetTurkeyLocationsAsync(1, 12);

            var vm = new WeatherViewModel.WeatherPageViewModel
            {
                CurrentLocation = location,
                CurrentForecast = forecast,
                FeaturedCities  = featuredCities,
                TurkeyLocations = turkeyItems,
                TurkeyTotalCount = turkeyTotal,
                TurkeyPage      = 1,
                TurkeyPageSize  = 12
            };

            return View(vm);
        }

        // ── 2. Şehir adıyla arama (JSON) ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> SearchLocation(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return BadRequest("En az 2 karakter girin.");

            var results = await _weatherService.SearchLocationAsync(query, 8);
            return Json(results);
        }

        // ── 3. Koordinata göre hava (JSON) ──────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetWeatherByLocation(double lat, double lon)
        {
            var forecast = await _weatherService.GetWeatherForecastAsync(lat, lon);
            if (forecast == null) return StatusCode(503, "Hava durumu verisi alınamadı.");
            return Json(forecast);
        }

        // ── 4. Türkiye Lokasyonları — sayfalı + aranabilir (JSON) ───────────
        [HttpGet]
        public async Task<IActionResult> GetTurkeyLocations(int page = 1, int pageSize = 12, string? search = null)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 6, 24);

            var (items, total) = await _weatherService.GetTurkeyLocationsAsync(page, pageSize, search);
            return Json(new
            {
                items,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        // ── 5. Dashboard AJAX için mevcut konum hava (JSON) ─────────────────
        [HttpGet]
        public async Task<IActionResult> GetCurrentLocationWeather()
        {
            var (location, forecast) = await _weatherService.GetWeatherByIpAsync();

            if (forecast == null)
                return StatusCode(503, "Hava durumu verisi alınamadı.");

            // Nem
            int? humidity = null;
            if (forecast.hourly?.time != null)
            {
                var now = DateTime.UtcNow;
                var idx = forecast.hourly.time
                    .Select((t, i) => (t, i))
                    .OrderBy(x => Math.Abs((DateTime.Parse(x.t) - now).TotalMinutes))
                    .FirstOrDefault().i;
                humidity = forecast.hourly.relativehumidity_2m?.ElementAtOrDefault(idx);
            }

            return Json(new
            {
                city    = location.City,
                country = location.CountryCode,
                temp    = forecast.current_weather?.temperature,
                windspeed = forecast.current_weather?.windspeed,
                weathercode = forecast.current_weather?.weathercode,
                isDay   = forecast.current_weather?.is_day,
                humidity,
                tempMax = forecast.daily?.temperature_2m_max?.ElementAtOrDefault(0),
                tempMin = forecast.daily?.temperature_2m_min?.ElementAtOrDefault(0)
            });
        }
    }
}
