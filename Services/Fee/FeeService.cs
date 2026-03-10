using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using Microsoft.EntityFrameworkCore;

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

        public async Task CreateAsync(FeeDto feeDto)
        {
            var entity = _mapper.Map<GarageFee>(feeDto);

            _context.GarageFees.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.GarageFees.FindAsync(id);

            if (entity != null)
            {
                _context.GarageFees.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}