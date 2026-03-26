using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        
    }
}
