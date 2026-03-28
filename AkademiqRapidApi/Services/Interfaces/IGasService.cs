using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IGasService
    {
        Task<List<GasViewModel.Result>> GetAllGasAsync();

        Task<List<GasViewModel.Result>> GetThreeGasAsync();
    }
}
