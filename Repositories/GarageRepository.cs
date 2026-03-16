using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Repositories
{
    public class GarageRepository : Repository<Garage>, IGarageRepository
    {
        public GarageRepository(GarageDbContext context) : base(context)
        {
        }

        public async Task<Garage?> GetGarageWithDetailsAsync(int id)
        {
            return await _context.Garages
                .Include(g => g.Location)
                .Include(g => g.PricingPolicy)
                    .ThenInclude(p => p.Rules)
                .Include(g => g.Sessions)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Garage?> FindAsync(System.Linq.Expressions.Expression<Func<Garage, bool>> predicate)
        {
            return await _context.Garages
                .Include(g => g.Location)
                .Include(g => g.PricingPolicy)
                    .ThenInclude(p => p.Rules)
                .Include(g => g.Sessions)
                .FirstOrDefaultAsync(predicate);
        }

        // ToDo
        public Task<IEnumerable<Garage>> GetWithinRadiusAsync(double latitude, double longitude, double radiusInKm)
        {
            throw new NotImplementedException();
        }
    }
}
