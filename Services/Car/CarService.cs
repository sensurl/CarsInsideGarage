using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Services.Car;
using CarsInsideGarage.Services.CarService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

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

        public void AddCar(CarDto carDto)
        {
            // 1. Translate DTO back into a Database Entity
            var carEntity = _mapper.Map<CarsInsideGarage.Data.Entities.Car>(carDto);

            // 2. Give the Entity to EF Core
            _context.Cars.Add(carEntity);

            // 3. Persist to DB
            _context.SaveChanges();
        }

        public void RemoveCar(int id)
        {

            var carEntity = _context.Cars.Find(id);
            if (carEntity != null)
            {
                _context.Cars.Remove(carEntity);
                _context.SaveChanges();
            }

            

        }
    }
}
