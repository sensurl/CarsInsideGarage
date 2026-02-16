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

        Task<int> CreateAsync(GarageCreateDto dto, string userId);


        Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id);
    }
}

