using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Car;
using CarsInsideGarage.Services.CarService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace CarsInsideGarage.Services.Car
{
    public class CarService : ICarService
    {
        private readonly Data.GarageDbContext _context;
        private readonly IMapper _mapper;
        public CarService(Data.GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<CarDto>> GetAllCarsAsync()
        {
            var cars = await _context.Cars.ToListAsync();

            return _mapper.Map<List<CarDto>>(cars);

        }

        public async Task<CarDto> GetCarByIdAsync(int id)
        {
            // Fetches the car where the ID matches. Returns null if not found.
            var carEntity = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carEntity == null)
            {
                // You could return null, or handle this with an exception
                return null; // "I searched, but found nothing."
            }

            return _mapper.Map<CarDto>(carEntity);
        }

        public async Task<int> AddCarAsync(CarDto carDto)
        {
            carDto.LicensePlate = carDto.LicensePlate.Trim().Replace(" ", "").ToUpper();

            // 1. Check the DB first (before doing any mapping work)
            var exists = await _context.Cars.AnyAsync(c => c.LicensePlate == carDto.LicensePlate);

            if (exists) throw new Exception("A car with this license plate already exists.");
            

            // 2. ONLY if it doesn't exist, do the mapping and saving
            var carEntity = _mapper.Map<CarsInsideGarage.Data.Entities.Car>(carDto);
             _context.Cars.Add(carEntity);
            await _context.SaveChangesAsync();
            return carEntity.Id;
        }
        
        public async Task<DeleteConfirmationViewModel> RemoveCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return null;

            // Capture the data BEFORE it's deleted
            var vm = new DeleteConfirmationViewModel
            {
                LicensePlate = car.LicensePlate,
                ExitTime = DateTime.Now
            };

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return vm; // Returning the "Ghost" to the Controller
        }
    }
}
