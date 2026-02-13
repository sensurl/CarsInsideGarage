using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Services.Garage
{
    public class GarageLocationService : IGarageLocationService
    {
        private readonly GarageDbContext _context;
        public GarageLocationService(GarageDbContext context)
        {
            _context = context;
        }
        public async Task<CarsInsideGarage.Data.Entities.Garage?> GetNearestAsync(double lat, double lng)
        {
            var garages = await _context.Garages
                .Include(g => g.Location)
                    .ThenInclude(l => l.Coordinates)
                .Include(g => g.Sessions)
                .Include(g => g.GarageFee)
                .ToListAsync();

            var nearest = garages
                .Select(g => new
                {
                    Garage = g,
                    Distance = Math.Sqrt(
                        Math.Pow((double)g.Location.Coordinates.Latitude - lat, 2) +
                        Math.Pow((double)g.Location.Coordinates.Longitude - lng, 2))
                })
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            return nearest?.Garage;
        }

    }
}
