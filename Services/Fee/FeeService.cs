using CarsInsideGarage.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CarsInsideGarage.Data;

namespace CarsInsideGarage.Services.Fee
{
    public class FeeService : IFeeService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;

        public FeeService(GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FeeDto>> GetAllAsync()
        {
            var fees = await _context.GarageFees.ToListAsync();

            return _mapper.Map<List<FeeDto>>(fees);
        }

        public async Task<FeeDto?> GetByIdAsync(int id)
        {
            // Returns null if not found.
            var feeEntity = await _context.GarageFees
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feeEntity == null)
            {
                return null; // "I searched, but found nothing."
            }

            return _mapper.Map<FeeDto>(feeEntity);
        }
    }
}