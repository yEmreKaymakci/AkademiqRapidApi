using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface ICoinService
    {
        Task<CoinViewModel.Rootobject> GetAllCoinsAsync();
        Task<List<CoinViewModel.Coin>> GetFeaturedCoinsAsync();
    }
}
