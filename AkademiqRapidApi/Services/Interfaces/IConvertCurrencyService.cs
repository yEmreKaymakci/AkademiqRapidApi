using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IConvertCurrencyService
    {
        Task<ConvertCurrencyViewModel.Rootobject> GetAllRatesAsync();
        Task<Dictionary<string, float>> GetMainRatesForDashboardAsync();
    }
}
