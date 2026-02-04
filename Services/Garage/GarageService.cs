using CarsInsideGarage.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CarsInsideGarage.Data;
using AutoMapper.QueryableExtensions;


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

        public async Task CreateAsync(
           string name,
           int capacity,
           int locationId,
           int garageFeeId)
        {
            var garage = new CarsInsideGarage.Data.Entities.Garage
            {
                Name = name,
                Capacity = capacity,
                LocationId = locationId,
                GarageFeeId = garageFeeId
            };

            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<GarageListDto>> GetAllAsync()
        {
            return await _context.Garages
                .ProjectTo<GarageListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
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
