using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;

namespace CarsInsideGarage.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GarageDbContext _context;

        public IGarageRepository Garages { get; }
        public IParkingSessionRepository Sessions { get; }

        public IRepository<Car> Cars { get; }


        public IRepository<Location> Locations { get; }

        public UnitOfWork(GarageDbContext context)
        {
            _context = context;

            Garages = new GarageRepository(_context);
            Sessions = new ParkingSessionRepository(_context);
            Cars = new Repository<Car>(_context);
            Locations = new Repository<Location>(_context);
        }

        public async Task<int> CompleteAsync()
        => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
