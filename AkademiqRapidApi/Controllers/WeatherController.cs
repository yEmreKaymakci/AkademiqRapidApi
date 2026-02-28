using AkademiqRapidApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AkademiqRapidApi.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WeatherController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var apiKey = _configuration["RapidApi:ApiKey"];
            var apiHost = _configuration["RapidApi:WeatherHost"];

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://{apiHost}/city?city=%C4%B0stanbul&lang=TR"),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                // .NET 9 ile gelen daha kısa ve performanslı deserialization
                var values = await response.Content.ReadFromJsonAsync<WeatherViewModel.Rootobject>();

                return View(values); // Veriyi View'a göndermeyi unutma!
            }
        }
    }
}
