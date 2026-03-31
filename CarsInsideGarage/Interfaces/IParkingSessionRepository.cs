
using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Interfaces
{
    public interface IParkingSessionRepository : IRepository<ParkingSession>
    {

        Task<ParkingSession?> GetActiveSessionByCarIdAsync(int carId);

        Task<IEnumerable<ParkingSession>> GetActiveSessionsByGarageIdAsync(int garageId);


        void Update(ParkingSession session);
    }

}
