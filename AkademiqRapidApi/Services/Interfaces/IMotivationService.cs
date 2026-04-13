using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IMotivationService
    {
        Task<QuoteViewModel.QuoteItem?> GetQuoteOfTheDayAsync();
        Task<List<QuoteViewModel.QuoteItem>> GetQuotesLibraryAsync();
    }
}
