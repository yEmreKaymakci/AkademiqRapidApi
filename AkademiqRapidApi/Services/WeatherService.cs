using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;

        // Sabit Türkiye şehir listesi (API'nin döndürdüğü popüler şehirler önden seçilmiştir;
        // cache sonrası dinamik arama da desteklenir)
        private static readonly (string city, double lat, double lon)[] _turkeySeeds = new[]
        {
            ("İstanbul",   41.0082, 28.9784),
            ("Ankara",     39.9208, 32.8541),
            ("İzmir",      38.4189, 27.1287),
            ("Bursa",      40.1826, 29.0669),
            ("Antalya",    36.8841, 30.7056),
            ("Adana",      37.0000, 35.3213),
            ("Konya",      37.8714, 32.4846),
            ("Gaziantep",  37.0662, 37.3833),
            ("Kayseri",    38.7312, 35.4787),
            ("Mersin",     36.8000, 34.6333),
            ("Eskişehir",  39.7767, 30.5206),
            ("Diyarbakır", 37.9144, 40.2306),
            ("Samsun",     41.2867, 36.3300),
            ("Trabzon",    41.0015, 39.7178),
            ("Erzurum",    39.9000, 41.2700),
            ("Malatya",    38.3552, 38.3095),
            ("Kocaeli",    40.7654, 29.9408),
            ("Şanlıurfa",  37.1591, 38.7969),
            ("Manisa",     38.6191, 27.4289),
            ("Balıkesir",  39.6484, 27.8826),
            ("Kahramanmaraş", 37.5858, 36.9371),
            ("Van",        38.4891, 43.4089),
            ("Hatay",      36.2021, 36.1603),
            ("Sakarya",    40.7731, 30.3948),
            ("Denizli",    37.7765, 29.0864),
            ("Muğla",      37.2153, 28.3636),
            ("Aydın",      37.8444, 27.8458),
            ("Tekirdağ",   40.9781, 27.5115),
            ("Afyon",      38.7507, 30.5567),
            ("Edirne",     41.6771, 26.5557),
            ("Sivas",      39.7477, 37.0179),
            ("Kastamonu",  41.3887, 33.7827),
            ("Zonguldak",  41.4564, 31.7987),
            ("Elazığ",     38.6810, 39.2264),
            ("Batman",     37.8812, 41.1351),
            ("Kırıkkale",  39.8468, 33.5153),
        };

        public WeatherService(HttpClient httpClient, IMemoryCache cache, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 1. Koordinata göre hava tahmini (ana metot, 15 günlük, caching'li)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<WeatherViewModel.ForecastRoot> GetWeatherForecastAsync(double lat, double lon)
        {
            // koordinatı 2 ondalığa yuvarlayarak cache anahtarı küçültülür
            var cacheKey = $"wx_{Math.Round(lat, 2)}_{Math.Round(lon, 2)}";
            if (_cache.TryGetValue(cacheKey, out WeatherViewModel.ForecastRoot cached))
                return cached;

            var url = BuildForecastUrl(lat, lon);
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var data = await _httpClient.GetFromJsonAsync<WeatherViewModel.ForecastRoot>(url, cts.Token);
                if (data is null) return null;

                _cache.Set(cacheKey, data, TimeSpan.FromMinutes(30));
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWeatherForecastAsync hata: lat={Lat} lon={Lon}", lat, lon);
                return null;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 2. Şehir adıyla geocoding arama
        // ─────────────────────────────────────────────────────────────────────
        public async Task<List<WeatherViewModel.LocationInfo>> SearchLocationAsync(string query, int count = 10)
        {
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(query)}&count={count}&language=tr&format=json";
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                var data = await _httpClient.GetFromJsonAsync<WeatherViewModel.LocationResult>(url, cts.Token);
                return data?.results ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SearchLocationAsync hata: query={Query}", query);
                return new();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 3. IP'ye göre konum + hava (ipapi.co/json, ücretsiz)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<(WeatherViewModel.IpLocationResult Location, WeatherViewModel.ForecastRoot Forecast)> GetWeatherByIpAsync()
        {
            const string ipCacheKey = "ip_location_default";

            WeatherViewModel.IpLocationResult location;
            if (!_cache.TryGetValue(ipCacheKey, out location))
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                    location = await _httpClient.GetFromJsonAsync<WeatherViewModel.IpLocationResult>(
                        "https://ipapi.co/json/", cts.Token);
                    // IP sonucu 10 dakika cache'le
                    if (location != null)
                        _cache.Set(ipCacheKey, location, TimeSpan.FromMinutes(10));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "IP geolocation başarısız, Ankara fallback kullanılıyor");
                }

                // Fallback — Ankara
                location ??= new WeatherViewModel.IpLocationResult
                {
                    City = "Ankara",
                    Region = "Ankara",
                    CountryName = "Türkiye",
                    CountryCode = "TR",
                    Latitude = 39.9208,
                    Longitude = 32.8541
                };
            }

            var forecast = await GetWeatherForecastAsync(location.Latitude, location.Longitude);
            return (location, forecast);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 4. Birden fazla şehri paralel çek → CityWeatherCard listesi
        // ─────────────────────────────────────────────────────────────────────
        public async Task<List<WeatherViewModel.CityWeatherCard>> GetWeatherForCitiesAsync(
            IEnumerable<(string city, string country)> cities)
        {
            // Her şehir için önce geocoding, sonra hava
            var tasks = cities.Select(async c =>
            {
                try
                {
                    var locations = await SearchLocationAsync(c.city, 1);
                    var loc = locations?.FirstOrDefault();
                    if (loc == null)
                        return new WeatherViewModel.CityWeatherCard { CityName = c.city, CountryCode = c.country, HasError = true };

                    var forecast = await GetWeatherForecastAsync(loc.latitude, loc.longitude);
                    return BuildCard(c.city, c.country, loc, forecast);
                }
                catch
                {
                    return new WeatherViewModel.CityWeatherCard { CityName = c.city, CountryCode = c.country, HasError = true };
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5. Türkiye lokasyonları — sabit liste + sayfalama + arama filtresi
        // ─────────────────────────────────────────────────────────────────────
        public async Task<(List<WeatherViewModel.CityWeatherCard> Items, int TotalCount)> GetTurkeyLocationsAsync(
            int page, int pageSize, string? search = null)
        {
            // 1. Listeyi filtrele
            var filtered = string.IsNullOrWhiteSpace(search)
                ? _turkeySeeds
                : _turkeySeeds.Where(t => t.city.Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            var totalCount = filtered.Length;
            var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToArray();

            // 2. Paralel hava tahminleri
            var tasks = paged.Select(async t =>
            {
                try
                {
                    var forecast = await GetWeatherForecastAsync(t.lat, t.lon);
                    return BuildCard(t.city, "TR", null, forecast, t.lat, t.lon);
                }
                catch
                {
                    return new WeatherViewModel.CityWeatherCard { CityName = t.city, CountryCode = "TR", HasError = true };
                }
            });

            var results = await Task.WhenAll(tasks);
            return (results.ToList(), totalCount);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Yardımcı metodlar
        // ─────────────────────────────────────────────────────────────────────
        private static string BuildForecastUrl(double lat, double lon) =>
            $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&current_weather=true" +
            $"&hourly=relativehumidity_2m" +
            $"&daily=temperature_2m_max,temperature_2m_min,weathercode,precipitation_probability_max,windspeed_10m_max,uv_index_max" +
            $"&timezone=auto" +
            $"&forecast_days=15";

        private static WeatherViewModel.CityWeatherCard BuildCard(
            string cityName,
            string countryCode,
            WeatherViewModel.LocationInfo loc,
            WeatherViewModel.ForecastRoot forecast,
            double? lat = null,
            double? lon = null)
        {
            if (forecast == null)
                return new WeatherViewModel.CityWeatherCard
                {
                    CityName = cityName,
                    CountryCode = countryCode,
                    HasError = true
                };

            // Mevcut saate en yakın hourly nem bul
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

            return new WeatherViewModel.CityWeatherCard
            {
                CityName = loc?.name ?? cityName,
                CountryCode = countryCode,
                Temperature = forecast.current_weather?.temperature,
                TempMax = forecast.daily?.temperature_2m_max?.ElementAtOrDefault(0),
                TempMin = forecast.daily?.temperature_2m_min?.ElementAtOrDefault(0),
                WeatherCode = forecast.current_weather?.weathercode ?? 0,
                WindSpeed = forecast.current_weather?.windspeed,
                Humidity = humidity,
                PrecipitationProbability = forecast.daily?.precipitation_probability_max?.ElementAtOrDefault(0),
                IsDay = (forecast.current_weather?.is_day ?? 1) == 1
            };
        }
    }
}