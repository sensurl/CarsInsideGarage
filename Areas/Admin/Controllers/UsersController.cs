using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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
