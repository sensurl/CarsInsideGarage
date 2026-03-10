using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGarageRepository Garages { get; }
        IParkingSessionRepository Sessions { get; }

        IRepository<Car> Cars { get; }
        IRepository<Location> Locations { get; }
       

        Task<int> CompleteAsync();
    }
}
