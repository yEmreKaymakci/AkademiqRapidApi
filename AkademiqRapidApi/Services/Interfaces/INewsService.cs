using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface INewsService
    {
        Task<List<NewsViewModel.Article>> GetNewsAsync(string category = "general");
    }
}
