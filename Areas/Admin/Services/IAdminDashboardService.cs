using CarsInsideGarage.Areas.Admin.Models;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
