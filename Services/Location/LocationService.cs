using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Services.Location
{
    public class LocationService : ILocationService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;

        public LocationService(GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllAsync()
        {
            var locations = await _context.Locations.ToListAsync();

            return _mapper.Map<List<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            // Returns null if not found.
            var locationEntity = await _context.Locations
                .FirstOrDefaultAsync(f => f.Id == id);

            if (locationEntity == null)
            {
                return null; // "I searched, but found nothing."
            }

            return _mapper.Map<LocationDto>(locationEntity);
        }
    }
}