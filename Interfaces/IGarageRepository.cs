using CarsInsideGarage.Data.Entities;
using System.Linq.Expressions;

namespace CarsInsideGarage.Interfaces
{
    public interface IGarageRepository : IRepository<Garage>
    {
        Task<Garage?> GetGarageWithDetailsAsync(int id);
        Task<Garage?> FindAsync(Expression<Func<Garage, bool>> predicate);

        Task<IEnumerable<Garage>> GetWithinRadiusAsync(double latitude, double longitude, double radiusInKm);


    }
}
