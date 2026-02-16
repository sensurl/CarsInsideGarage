using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Services.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllCarsAsync();
        Task<CarDto> GetCarByIdAsync(int id);

        // Get the DTO from the UI/Controller
        Task<int> AddCarAsync(CarDto carDto);

        Task<CarDeleteConfirmationViewModel> RemoveCarAsync(int id);
    }
}
