using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Services.Garage
{
 

        public interface IGarageLocationService
        {
            Task<CarsInsideGarage.Data.Entities.Garage?> GetNearestAsync(double lat, double lng);

            Task<IEnumerable<CarsInsideGarage.Data.Entities.Garage?>> GetNearestManyAsync(double lat, double lng, int count);
        }

    
}
