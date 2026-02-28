using AkademiqRapidApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AkademiqRapidApi.Controllers
{
    public class ConvertCurrencyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ConvertCurrencyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var apiKey = _configuration["RapidApi:ApiKey"];
            var apiHost = _configuration["RapidApi:CurrencyHost"];

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://{apiHost}/api/v1/convert?amount=1&base_currency=EUR&quote_currency=TRY"),
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
                var values = await response.Content.ReadFromJsonAsync<ConvertCurrencyViewModel.Rootobject>();

                return View(values); // Veriyi View'a göndermeyi unutma!
            }
        }
    }
}
