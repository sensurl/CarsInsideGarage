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

        public async Task<IActionResult> ListUsers()
            {
            // Use .IgnoreQueryFilters() so the Admin can see the deleted users to restore them
            var users = await _userManager.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsDeleted) // Maybe only show deleted ones in this view?
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

            return RedirectToAction(nameof(Index));
            }

       

    }

    }
