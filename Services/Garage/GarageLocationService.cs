using CarsInsideGarage.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CarsInsideGarage.Services.Garage
{
    public class GarageLocationService : IGarageLocationService
    {
        private readonly GarageDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public GarageLocationService(GarageDbContext context)
        {
            _context = context;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        public async Task<Data.Entities.Garage?> GetNearestAsync(double lat, double lng)
        {
            var userLocation = _geometryFactory.CreatePoint(new Coordinate(lng, lat));

            return await _context.Garages
                .Include(g => g.Location)
                .Where(g => g.Location.ParkingCoordinates != null)
                .OrderBy(g => g.Location.ParkingCoordinates.Distance(userLocation))
                .FirstOrDefaultAsync();
        }
    }
}
