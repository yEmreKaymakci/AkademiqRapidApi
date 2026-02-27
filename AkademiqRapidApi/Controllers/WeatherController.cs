using AkademiqRapidApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AkademiqRapidApi.Controllers
{
    public class WeatherController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://open-weather13.p.rapidapi.com/city?city=%C4%B0stanbul&lang=TR"),
                Headers =
    {
        { "x-rapidapi-key", "eaa8321078msh8aa48935ef5e1d8p1cb46fjsn6493e1885e8e" },
        { "x-rapidapi-host", "open-weather13.p.rapidapi.com" },
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
