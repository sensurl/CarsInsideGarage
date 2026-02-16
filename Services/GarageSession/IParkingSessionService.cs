using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Services.GarageSession
{
    public interface IParkingSessionService
    {
        Task StartSessionAsync(int garageId, int carId);
        Task EndSessionAsync(int sessionId);
        Task<IEnumerable<SessionActiveViewModel>> GetActiveSessionsAsync(int garageId);
        Task PayAsync(int sessionId, decimal amount);
        Task<SessionActiveViewModel?> GetActiveSessionByCarAsync(int carId);
        Task<int> GetCarIdBySessionId(int sessionId);
    }
}