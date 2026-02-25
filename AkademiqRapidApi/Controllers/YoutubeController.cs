using AkademiqRapidApi.Models;
using Microsoft.AspNetCore.Mvc;

public class YoutubeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public YoutubeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return View();

        var apiKey = _configuration["RapidApi:ApiKey"];
        var apiHost = _configuration["RapidApi:YoutubeHost"];

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://{apiHost}/dl?id={id}"),
            Headers =
            {
                { "x-rapidapi-key", apiKey },
                { "x-rapidapi-host", apiHost },
            },
        };

        using (var response = await client.SendAsync(request))
        {
            if (response.IsSuccessStatusCode)
            {
                var values = await response.Content.ReadFromJsonAsync<YoutubeViewModel.Rootobject>();
                return View(values);
            }

            // 403 veya diğer hatalar için ViewBag ile uyarı verebilirsin
            ViewBag.Error = "API isteği başarısız oldu. Lütfen ID'yi kontrol edin.";
            return View();
        }
    }
}