using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Services.Garage
{
    public interface IGarageLocationService
    {
        Task<CarsInsideGarage.Data.Entities.Garage?> GetNearestAsync(double lat, double lng);
    }
}
