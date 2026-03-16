using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Repositories;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGarageService _garageService;
        private readonly IParkingSessionService _parkingSessionService;

        public AdminDashboardService(IGarageService garageService, IParkingSessionService parkingSessionService)
        {
            _garageService = garageService;
            _parkingSessionService = parkingSessionService;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var garages = await _garageService.GetAllAsync(new CurrentUser { IsAdmin = true });

            // 1. Get all active sessions globally
            var activeSessions = await _parkingSessionService.GetAllActiveSessionsAsync();

            return new DashboardStatsDto
            {
                TotalGarages = garages.Count(),
                TotalFreeSpotsAvailable = garages.Sum(g => g.FreeSpots),
                TotalActiveSessions = activeSessions.Count()
            };
        }
    }
}
