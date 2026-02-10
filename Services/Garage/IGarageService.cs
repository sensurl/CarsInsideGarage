using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarsInsideGarage.Services.Garage
{
    public interface IGarageService
    {
        Task<IEnumerable<GarageListDto>> GetAllAsync();
        Task<GarageDetailsViewModel?> GetDetailsViewModelAsync(int garageId, bool isOwner);

        Task CreateAsync(
            string name,
            int capacity,
            Area area,
            string coordinates,
            int garageFeeId);

       
        Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id);
    }
}

