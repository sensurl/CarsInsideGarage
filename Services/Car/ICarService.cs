using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarsInsideGarage.Services.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllCarsAsync(CurrentUser user);
        Task<CarDto?> GetCarByIdAsync(int id, CurrentUser user);

        // Get the DTO from the UI/Controller
        Task<int> AddCarAsync(CarDto carDto, CurrentUser user);

        Task<CarDeleteConfirmationViewModel?> RemoveCarAsync(int id, CurrentUser user);

        // Returns true if the car was successfully restored, false otherwise (e.g., if the car doesn't exist or the user isn't authorized).
        Task<bool> RestoreCarAsync(int id, CurrentUser user);
    }
}
