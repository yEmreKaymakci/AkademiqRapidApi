using System.Text.Json.Serialization;

namespace AkademiqRapidApi.Models
{
    public class WeatherViewModel
    {
        // ── Geocoding ──────────────────────────────────────────────────────────
        public class LocationResult
        {
            public List<LocationInfo>? results { get; set; }
        }

        public class LocationInfo
        {
            public int id { get; set; }
            public string? name { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string? country { get; set; }
            public string? country_code { get; set; }
            public string? admin1 { get; set; }
            public int? population { get; set; }
        }

        // ── IP Geolocation (ipapi.co/json) ────────────────────────────────────
        public class IpLocationResult
        {
            [JsonPropertyName("city")]
            public string? City { get; set; }

            [JsonPropertyName("region")]
            public string? Region { get; set; }

            [JsonPropertyName("country_name")]
            public string? CountryName { get; set; }

            [JsonPropertyName("country_code")]
            public string? CountryCode { get; set; }

            [JsonPropertyName("latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double Longitude { get; set; }
        }

        // ── Open-Meteo Forecast ────────────────────────────────────────────────
        public class ForecastRoot
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string? timezone { get; set; }
            public CurrentWeather? current_weather { get; set; }
            public HourlyData? hourly { get; set; }
            public DailyForecast? daily { get; set; }
        }

        public class CurrentWeather
        {
            public double temperature { get; set; }
            public double windspeed { get; set; }
            public int weathercode { get; set; }
            public int is_day { get; set; }
            public string? time { get; set; }
        }

        public class HourlyData
        {
            public List<string>? time { get; set; }
            public List<int>? relativehumidity_2m { get; set; }
        }

        public class DailyForecast
        {
            public List<string>? time { get; set; }
            public List<double>? temperature_2m_max { get; set; }
            public List<double>? temperature_2m_min { get; set; }
            public List<int>? weathercode { get; set; }
            public List<int?>? precipitation_probability_max { get; set; }
            public List<double?>? windspeed_10m_max { get; set; }
            public List<double?>? uv_index_max { get; set; }
        }

        // ── City Card (4 dünya şehri + Türkiye listesi kartları) ──────────────
        public class CityWeatherCard
        {
            public string? CityName { get; set; }
            public string? CountryCode { get; set; }
            public string? LocalTime { get; set; }
            public double? Temperature { get; set; }
            public double? TempMax { get; set; }
            public double? TempMin { get; set; }
            public int WeatherCode { get; set; }
            public double? WindSpeed { get; set; }
            public int? Humidity { get; set; }
            public int? PrecipitationProbability { get; set; }
            public bool IsDay { get; set; }
            public bool HasError { get; set; }
        }

        // ── Birleşik sayfa modeli (Weather/Index) ─────────────────────────────
        public class WeatherPageViewModel
        {
            public IpLocationResult? CurrentLocation { get; set; }
            public ForecastRoot? CurrentForecast { get; set; }
            public List<CityWeatherCard> FeaturedCities { get; set; } = new();
            public List<CityWeatherCard> TurkeyLocations { get; set; } = new();
            public int TurkeyTotalCount { get; set; }
            public int TurkeyPage { get; set; } = 1;
            public int TurkeyPageSize { get; set; } = 12;
        }
    }
}
