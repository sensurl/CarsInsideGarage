using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.CarService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Runtime.ConstrainedExecution;

namespace CarsInsideGarage.Services.Car
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
       // private readonly GeometryFactory _geometryFactory = 
        //    NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        public CarService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
         }

        // ================================
        // LIST
        // ================================

        public async Task<IEnumerable<CarDto>> GetAllCarsAsync(CurrentUser user)
        {
            var cars = await _unitOfWork.Cars.GetAllAsync();

            // If not an Admin, only show own cars
            cars = user.IsAdmin 
                ? cars 
                : cars.Where(c => c.UserId == user.UserId);

            // Project to DTO and return the result
            return _mapper.Map<List<CarDto>>(cars);
        }

        // ================================
        // SEARCH
        // ================================

        public async Task<CarDto?> GetCarByIdAsync(int id, CurrentUser user)
        {
            // Fetches the car where the ID matches. Returns null if not found.
            var carEntity = await _unitOfWork.Cars
                .GetByIdAsync(id);

            if (carEntity == null)
            {
                // Could return null, or handle this with an exception
                return null; // "I searched, but found nothing."
            }

            // Authorization rule:
            if (!user.IsAdmin && carEntity.UserId != user.UserId)
                return null; // or throw UnauthorizedAccessException

            return _mapper.Map<CarDto>(carEntity);
        }


        // ================================
        // CREATE
        // ================================

        public async Task<int> AddCarAsync(CarDto carDto, CurrentUser user)
        {
            if (string.IsNullOrEmpty(user.UserId))
                throw new Exception("You must be logged in to register a car.");

            // 1. Clean up the plate number
            carDto.CarPlateNumber = carDto.CarPlateNumber.Trim().Replace(" ", "").ToUpper();

            
            // 2. Check for duplicates
            var existingCars = await _unitOfWork.Cars
                .GetAllAsync();

            var exists = existingCars
                .Any(c => c.CarPlateNumber == carDto.CarPlateNumber);

            if (exists) 
                throw new Exception("A car with this license plate already exists.");

            var carEntity = _mapper.Map<CarsInsideGarage.Data.Entities.Car>(carDto);

            carEntity.UserId = user.UserId;

            await _unitOfWork.Cars.AddAsync(carEntity);
            await _unitOfWork.CompleteAsync();

            return carEntity.Id;
        }


        // ================================
        // DELETE
        // ================================

        public async Task<CarDeleteConfirmationViewModel?> RemoveCarAsync(int id, CurrentUser user)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(id);
            
            if (car == null) 
                return null;

            // Driver can delete ONLY own car
            if (user.IsDriver && car.UserId != user.UserId)
                return null;

            // Admin can delete ANY car
            if (!user.IsDriver && !user.IsAdmin)
                return null;

            // Capture the data BEFORE it's deleted
            var vm = new CarDeleteConfirmationViewModel
            {
                CarPlateNumber = car.CarPlateNumber,
                ExitTime = DateTime.UtcNow
            };

            if (user.IsAdmin)
            {
                // HARD DELETE
                _unitOfWork.Cars.Remove(car);
            }
            else
            {
                // SOFT DELETE
                car.IsDeleted = true;
                car.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.CompleteAsync();

            // Returning the "Ghost" of the deleted Car to the Controller
            return vm; 
        }

        public async Task<bool> RestoreCarAsync(int id, CurrentUser user)
        {
            // Only Admin can restore

            if (!user.IsAdmin)
                return false;

            var car = await _unitOfWork.Cars
                .GetByIdIncludingDeletedAsync(id);

            if (car == null || !car.IsDeleted)
                return false;

            car.IsDeleted = false;
            car.DeletedAt = null;

            await _unitOfWork.CompleteAsync();

            return true;
        }


    }
}
