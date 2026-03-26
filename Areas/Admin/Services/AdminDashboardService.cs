using CarsInsideGarage.Areas.Admin.Models;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Repositories;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Areas.Admin.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGarageService _garageService;
        private readonly IParkingSessionService _parkingSessionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardService(IGarageService garageService, IParkingSessionService parkingSessionService, UserManager<ApplicationUser> userManager)
        {
            _garageService = garageService;
            _parkingSessionService = parkingSessionService;
            _userManager = userManager;

        }

        // ================================
        // STATISTICS
        // ================================

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var garages = await _garageService.GetAllAsync(new CurrentUser { IsAdmin = true });
            var activeSessions = await _parkingSessionService.GetAllActiveSessionsAsync();
            var deletedGarages = await _garageService.GetDeletedAsync();
            var deletedUsers = await _userManager.Users.IgnoreQueryFilters().Where(u => u.IsDeleted).ToListAsync();

            return new DashboardStatsDto
            {
                TotalGarages = garages.Count(),
                TotalFreeSpotsAvailable = garages.Sum(g => g.FreeSpots),
                TotalActiveSessions = activeSessions.Count(),
                DeletedGaragesCount = deletedGarages.Count(),
                DeletedUsersCount = deletedUsers.Count()
            };
        }


        // ================================
        // GARAGES - VIEW DELETED & RESTORE
        // ================================


        public async Task<IEnumerable<CarsInsideGarage.Data.Entities.Garage?>> GetDeletedGaragesAsync()
        {
            return await _garageService.GetDeletedAsync(); 
        }

        public async Task<bool> RestoreGarageAsync(int garageId)
        {
            
            return await _garageService.RestoreAsync(garageId);
        }

        // ================================
        // USERS - RESTORE
        // ================================

        public async Task<bool> RestoreUserAsync(string userId, UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null || !user.IsDeleted)
                return false;

            user.IsDeleted = false;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }


    }
}
