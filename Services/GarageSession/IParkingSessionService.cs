using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Services.GarageSession
{
    public interface IParkingSessionService
    {
        Task StartSessionAsync(int garageId, string userId);
        Task EndSessionAsync(int sessionId, string userId);
        Task PayAsync(int sessionId, decimal amount, string userId);
        Task<bool> StopSessionAsync(int sessionId, CurrentUser user);
       
        // Zero Client trust methods
        Task<SessionActiveViewModel?> GetActiveSessionForDriverAsync(string userId);

        // for Owner
        Task<IEnumerable<ActiveSessionListViewModel>> GetActiveSessionsForGarageOwnerAsync(string userId);

        // for Admin dashboard
        Task<IEnumerable<ParkingSession>> GetAllActiveSessionsAsync();

        // internal usage only
        Task<int> GetCarIdBySessionIdAsync(int sessionId); 
    }
}