using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Repositories
{
    public class ParkingSessionRepository : Repository<ParkingSession>, IParkingSessionRepository
    {
        public ParkingSessionRepository(GarageDbContext context) : base(context)
        {
        }

         public async Task<ParkingSession?> GetActiveSessionByCarIdAsync(int carId)
        {
            return await _context.Set<ParkingSession>()
                .Include(s => s.Car)
                .Include(s => s.Garage)
                .FirstOrDefaultAsync(s => s.CarId == carId && s.ExitTime == null);
        }

        public async Task<IEnumerable<ParkingSession>>
            GetActiveSessionsByGarageIdAsync(int garageId)
        {
            return await _context.Set<ParkingSession>()
                .Include(s => s.Car)
                .Include(s => s.Garage)
                .Where(s => s.GarageId == garageId && s.ExitTime == null)
                .ToListAsync();
        }


        public void Update(ParkingSession session)
        {
            _context.Set<ParkingSession>()
                .Update(session);
        }



    }
}
