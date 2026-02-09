using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace CarsInsideGarage.Services.Garage
{
    public class GarageService : IGarageService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;
        public GarageService(GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GarageListDto>> GetAllAsync()
        {
            return await _context.Garages
                .ProjectTo<GarageListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task CreateAsync(string name, int capacity, Area area, string coordinates, int feeId)
        {
            // 1. Create the new Location entity first
            var newLocation = new CarsInsideGarage.Data.Entities.Location
            {
                Area = area,
                AddressCoordinates = coordinates
            };

            // 2. Add Location to context
            _context.Locations.Add(newLocation);

            // We save here so newLocation.Id is populated by the DB
            await _context.SaveChangesAsync();

            // 3. Create the Garage using the new Location's ID
            var newGarage = new CarsInsideGarage.Data.Entities.Garage
            {
                Name = name,
                Capacity = capacity,
                LocationId = newLocation.Id, // Linking the One-to-One relationship
                GarageFeeId = feeId
            };

            _context.Garages.Add(newGarage);
            await _context.SaveChangesAsync();
        }

        public async Task<GarageDetailsDto?> GetGarageDetailsAsync(int id)
        {
            return await _context.Garages
                .Where(g => g.Id == id)
                .ProjectTo<GarageDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

    }
}