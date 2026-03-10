using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Services.Garage;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGarageService _garageService;

        public AdminDashboardService(IGarageService garageService)
        {
            _garageService = garageService;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            
                var garages = await _garageService.GetAllAsync(
                    new CarsInsideGarage.Models.Auth.CurrentUser { IsAdmin = true });

                var dto = new DashboardStatsDto
                {
                    TotalGarages = garages.Count(),
                    TotalFreeSpotsAvailable = garages.Sum(g => g.FreeSpots)
                };

                return dto;
           
        }
    }
}
