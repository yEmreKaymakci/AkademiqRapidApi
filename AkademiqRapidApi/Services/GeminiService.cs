using AkademiqRapidApi.Models;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AkademiqRapidApi.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _client;
        private readonly GeminiOptions _geminiOptions;
        private readonly IServiceProvider _serviceProvider; 

        public GeminiService(HttpClient client, IOptions<GeminiOptions> geminiOptions, IServiceProvider serviceProvider)
        {
            _client = client;
            _geminiOptions = geminiOptions.Value;
            _serviceProvider = serviceProvider; 
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            return "Async kullanilmiyor, stream calisiyor.";
        }

        public async IAsyncEnumerable<string> GetChatResponseStreamAsync(string userMessage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var apiKey = _geminiOptions.ApiKey;
            var apiUrl = _geminiOptions.StreamApiUrl;

            var requestUrl = apiUrl + apiKey;
            
            string systemPrompt = await BuildSystemContextAsync();

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = userMessage
                            }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = jsonContent };

            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("data: "))
                {
                    var jsonData = line.Substring("data: ".Length);
                    string? textChunk = null;
                    bool isValid = false;

                    try
                    {
                        var jsonDocument = JsonDocument.Parse(jsonData);
                        textChunk = jsonDocument.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                        isValid = !string.IsNullOrEmpty(textChunk);
                    }
                    catch (Exception)
                    {
                        isValid = false;
                    }

                    if (isValid && textChunk != null)
                    {
                        yield return textChunk;
                    }
                }
            }
        }

        private async Task<string> BuildSystemContextAsync()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Senin adın 'Asistan Nova'. Sen AkademiqRapidApi sisteminin gelişmiş, siberpunk temalı yapay zeka asistanısın.");
            sb.AppendLine("Kullanıcıların proje genelindeki mevcut API entegrasyonlarına dair sorularına profesyonel ve kısa yanıtlar vereceksin.");
            sb.AppendLine("\n-- ŞU ANKİ SİSTEM VERİLERİ ÖZETİ --");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                
                var newsService = scope.ServiceProvider.GetService<INewsService>();
                if (newsService != null)
                {
                    var news = await newsService.GetNewsAsync();
                    if(news != null && news.Any())
                        sb.AppendLine($"- Son Haber Başlığı: {news.First().Title}");
                }

                var gasService = scope.ServiceProvider.GetService<IGasService>();
                if (gasService != null)
                {
                    var gas = await gasService.GetThreeGasAsync();
                    if(gas != null && gas.Any())
                        sb.AppendLine($"- Akaryakıt Verisi (Örnek): {gas.First().gasoline} TL Benzin fiyatı mevcut.");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"- Veri çekilirken hata oluştu: {ex.Message}");
            }

            sb.AppendLine("\nYanıtlarken Asistan Nova karakterini bozma ve çok uzun cevaplar verme. Kısa ve öz konuş.");
            return sb.ToString();
        }
    }
}
