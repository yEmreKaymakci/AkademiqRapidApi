using AkademiqRapidApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AkademiqRapidApi.Controllers
{
    public class ConvertCurrencyController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://fast-price-exchange-rates.p.rapidapi.com/api/v1/convert?amount=1&base_currency=EUR&quote_currency=TRY"),
                Headers =
    {
        { "x-rapidapi-key", "eaa8321078msh8aa48935ef5e1d8p1cb46fjsn6493e1885e8e" },
        { "x-rapidapi-host", "fast-price-exchange-rates.p.rapidapi.com" },
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
