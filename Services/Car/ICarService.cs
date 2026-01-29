using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using System.Collections.Generic;

namespace CarsInsideGarage.Services.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllCarsAsync();
        Task<CarDto> GetCarByIdAsync(int id);

        // Accept the DTO from the UI/Controller
        Task<int> AddCarAsync(CarDto carDto);

        Task<DeleteConfirmationViewModel> RemoveCarAsync(int id);
    }
}
