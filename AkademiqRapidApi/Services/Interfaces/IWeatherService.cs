using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IWeatherService
    {
        /// <summary>Koordinata göre 15 günlük + anlık tahmin (caching'li)</summary>
        Task<WeatherViewModel.ForecastRoot> GetWeatherForecastAsync(double lat, double lon);

        /// <summary>Geocoding — şehir adıyla arama (max 10 sonuç)</summary>
        Task<List<WeatherViewModel.LocationInfo>> SearchLocationAsync(string query, int count = 10);

        /// <summary>Ziyaretçinin IP'sinden konum + hava tahmini döndürür</summary>
        Task<(WeatherViewModel.IpLocationResult Location, WeatherViewModel.ForecastRoot Forecast)> GetWeatherByIpAsync();

        /// <summary>Birden fazla şehrin hava kartını paralel çeker</summary>
        Task<List<WeatherViewModel.CityWeatherCard>> GetWeatherForCitiesAsync(IEnumerable<(string city, string country)> cities);

        /// <summary>Türkiye şehirlerini sayfalı ve aranabilir getirir</summary>
        Task<(List<WeatherViewModel.CityWeatherCard> Items, int TotalCount)> GetTurkeyLocationsAsync(
            int page, int pageSize, string? search = null);
    }
}