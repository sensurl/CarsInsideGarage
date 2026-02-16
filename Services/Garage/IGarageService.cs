using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Services.Garage
{
    public interface IGarageService
    {
        Task<IEnumerable<GarageListDto>> GetAllAsync();
        Task<GarageDetailsViewModel?> GetDetailsViewModelAsync(int garageId, bool isOwner);
        Task<int> CreateAsync(GarageCreateDto dto, string userId);
        Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id);
    }
}