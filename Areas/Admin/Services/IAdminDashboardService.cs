using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<IEnumerable<CarsInsideGarage.Data.Entities.Garage?>> GetDeletedGaragesAsync();
        Task<bool> RestoreGarageAsync(int garageId);

        Task<bool> RestoreUserAsync(string userId, UserManager<ApplicationUser> userManager);
    }
}
