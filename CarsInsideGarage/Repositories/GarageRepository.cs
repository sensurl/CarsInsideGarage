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

   
        public async Task<IEnumerable<Garage>> GetWithinRadiusAsync(
    double latitude,
    double longitude,
    double radiusInKm)
        {
            var point = new NetTopologySuite.Geometries.Point(longitude, latitude) { SRID = 4326 };

            return await _context.Garages
                .Where(g => g.Location.ParkingCoordinates.Distance(point) <= radiusInKm * 1000)
                .Include(g => g.Location)
                .ToListAsync();
        }

        public async Task<Garage?> GetNearestAsync(double lat, double lng)
        {
            var point = new NetTopologySuite.Geometries.Point(lng, lat) { SRID = 4326 };

            return await _context.Garages
                .Include(g => g.Location)
                .OrderBy(g => g.Location.ParkingCoordinates.Distance(point))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Garage>> GetNearestManyAsync(double lat, double lng, int count)
        {
            var point = new NetTopologySuite.Geometries.Point(lng, lat) { SRID = 4326 };

            return await _context.Garages
                .Include(g => g.Location)
                .Include(g => g.Sessions)
                .OrderBy(g => g.Location.ParkingCoordinates.Distance(point))
                .Take(count)
                .ToListAsync();
        }

    }
}
