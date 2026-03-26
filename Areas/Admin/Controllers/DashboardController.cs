using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Areas.Admin.Services;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Areas.Admin.Controllers
    {
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
        {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public DashboardController(IAdminDashboardService adminDashboardService, UserManager<ApplicationUser> userManager)
            {
            _adminDashboardService = adminDashboardService;
            _userManager = userManager;
            }


        // ================================
        // STATISTICS
        // ================================

        public async Task<IActionResult> Index()
            {
            var statsDto = await _adminDashboardService.GetDashboardStatsAsync();

            // Map Dto to ViewModel
            var viewModel = new DashboardViewModel
                {
                TotalGarages = statsDto.TotalGarages,
                TotalFreeSpotsAvailable = statsDto.TotalFreeSpotsAvailable,
                TotalActiveSessions = statsDto.TotalActiveSessions
                };

            return View(viewModel);
            }


        // ================================
        // USERS
        // ================================

        public async Task<IActionResult> ListDeletedUsers()
            {
            // See the deleted users to restore them
            var users = await _userManager.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsDeleted) 
                .ToListAsync();

            return View(users);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreUser(string userId)
            {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            if (user.IsDeleted)
                {
                user.IsDeleted = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    {
                    // Handle error (e.g., add to ModelState)
                    return RedirectToAction(nameof(Index));
                    }
                }

            return RedirectToAction(nameof(ListDeletedUsers));
            }

        // ================================
        // GARAGES
        // ================================

        public async Task<IActionResult> ListDeletedGarages()
        {
            var garages = await _adminDashboardService.GetDeletedGaragesAsync();
            return View(garages); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreGarage(int garageId)
        {
            var restored = await _adminDashboardService.RestoreGarageAsync(garageId);

            if (!restored)
                return NotFound();

            return RedirectToAction(nameof(ListDeletedGarages));
        }

        

    }

}
