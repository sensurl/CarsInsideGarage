using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarsInsideGarage.Areas.Admin.Services;

namespace CarsInsideGarage.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        public DashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _adminDashboardService.GetDashboardStatsAsync();
            return View(stats);
        }
        
    }
}
