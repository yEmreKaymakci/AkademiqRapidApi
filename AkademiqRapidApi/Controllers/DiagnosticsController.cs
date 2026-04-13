using Microsoft.AspNetCore.Mvc;

namespace AkademiqRapidApi.Controllers
{
    /// <summary>
    /// Geliştirme ortamı için API test endpoint'leri.
    /// </summary>
    public class DiagnosticsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public DiagnosticsController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // GET /Diagnostics/Football?path=matches
        [HttpGet]
        public async Task<IActionResult> Football(string path = "matches")
        {
            var apiKey = _configuration["FootballDataOrg:ApiKey"] ?? "";
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Auth-Token", apiKey);

            var url = $"https://api.football-data.org/v4/{path}";

            try
            {
                var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                return Content(
                    $"URL: {url}\nStatus: {(int)response.StatusCode} {response.StatusCode}\n" +
                    $"Key prefix: {(apiKey.Length > 6 ? apiKey[..6] : "(YOK)")}...\n\n{body}",
                    "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"URL: {url}\nHATA: {ex.Message}", "text/plain");
            }
        }

        // GET /Diagnostics/Info
        [HttpGet]
        public IActionResult Info()
        {
            string today;
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).ToString("yyyy-MM-dd");
            }
            catch { today = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd"); }

            var fdoKey   = _configuration["FootballDataOrg:ApiKey"] ?? "(YOK)";

            return Content(
                $"UTC Now  : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n" +
                $"TR Bugün : {today}\n" +
                $"FDO Key  : {(fdoKey.Length > 6 ? fdoKey[..6] : fdoKey)}...\n\n" +
                $"Test URL'leri:\n" +
                $"  /Diagnostics/Football?path=matches           (bugünün maçları)\n" +
                $"  /Diagnostics/Football?path=matches?status=IN_PLAY,PAUSED  (canlı)\n" +
                $"  /Diagnostics/Football?path=competitions/2021/matches?dateFrom={today}&dateTo={today}  (PL)\n",
                "text/plain");
        }
    }
}
