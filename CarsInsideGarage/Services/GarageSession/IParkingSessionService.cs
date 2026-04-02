using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.ViewModels;
using System.Collections;

namespace CarsInsideGarage.Services.GarageSession
{
    public interface IParkingSessionService
    {
        Task StartSessionAsync(int garageId, int carId, string userId);
        Task EndSessionAsync(int sessionId, string userId);
        Task PayAsync(int sessionId, decimal amount, string userId);
        Task<bool> StopSessionAsync(int sessionId, CurrentUser user);

        // Zero Client trust methods
        // A Driver has one Car only
        Task<SessionActiveViewModel?> GetActiveSessionDetailsAsync(int sessionId, string userId);

        // A Driver has multiple Cars
        Task<IEnumerable<SessionActiveViewModel?>> GetActiveSessionsForDriverAsync(string userId);

        // for Owner
        Task<IEnumerable<ActiveSessionListViewModel>> GetActiveSessionsForGarageOwnerAsync(string userId);

        // for Admin dashboard
        Task<IEnumerable<ParkingSession>> GetAllActiveSessionsAsync();

        // internal usage only
        Task<int> GetCarIdBySessionIdAsync(int sessionId); 
    }
}