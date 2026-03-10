using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Services.Garage
{
    public interface IGarageService
    {
        Task<IEnumerable<GarageListDto>> GetAllAsync(CurrentUser user);
        Task<GarageDetailsViewModel?> GetDetailsViewModelAsync(int garageId, CurrentUser user);
        Task<int> CreateAsync(GarageCreateDto dto, CurrentUser user);
        Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id, CurrentUser user);
    }
}