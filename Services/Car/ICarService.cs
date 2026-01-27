using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using System.Collections.Generic;

namespace CarsInsideGarage.Services.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllCarsAsync();
        Task<CarDto> GetCarByIdAsync(int id);

        // Accept the DTO from the UI/Controller
        void AddCar(CarDto carDto);

        void RemoveCar(int id);
    }
}
