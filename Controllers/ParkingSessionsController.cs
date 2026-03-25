using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarsInsideGarage.Controllers
{
    [Authorize]
    public class ParkingSessionsController : Controller
    {
        private readonly IParkingSessionService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParkingSessionsController(IParkingSessionService service, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
        }

        // =====================================================
        // HELPERS
        // =====================================================

        private string GetUserId()
            => _userManager.GetUserId(User)!;

        private CurrentUser BuildCurrentUser()
        {
            return new CurrentUser
            {
                UserId = GetUserId(),
                IsAdmin = User.IsInRole("Admin"),
                IsOwner = User.IsInRole("GarageOwner"),
                IsDriver = User.IsInRole("Driver")
            };
        }

        // ==========================================
        // DRIVER
        // ==========================================

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> MySessions()
        {
            var sessions = await _service.GetActiveSessionsForDriverAsync(GetUserId());

            if (!sessions.Any())
                return View("NoActiveSession");

            return View(sessions);
        }

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Active(int? id)
        {
            if (id == null) return RedirectToAction(nameof(MySessions));

            // Uses your existing service method GetActiveSessionDetailsAsync
            var session = await _service.GetActiveSessionDetailsAsync(id.Value, GetUserId());

            if (session == null)
                return View("NoActiveSession");

            return View(session);
        }


        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Start(int garageId, int? carId)
        {
            try
            {
                // If carId is null, the user clicked "Park" from the Garage list
                if (carId == null)
                {
                    // We need to check how many cars the user has. 
                    // You can use your existing _unitOfWork or a Service call here.
                    var userId = GetUserId();
                    var cars = await _userManager.Users
                        .Include(u => u.Cars)
                        .Where(u => u.Id == userId)
                        .SelectMany(u => u.Cars)
                        .Where(c => !c.IsDeleted)
                        .ToListAsync();

                    if (cars.Count == 0)
                    {
                        TempData["Error"] = "Please register a car first.";
                        return RedirectToAction("Index", "Cars");
                    }

                    if (cars.Count > 1)
                    {
                        // REDIRECT TO SELECT CAR VIEW
                        var vm = new SelectCarViewModel
                        {
                            GarageId = garageId,
                            AvailableCars = _mapper.Map<IEnumerable<CarSelectionDto>>(cars)
                        };
                        return View("SelectCar", vm);
                    }

                    // If only one car, take its ID
                    carId = cars.First().Id;
                }

                // Call the service with the specific carId
                await _service.StartSessionAsync(garageId, carId.Value, GetUserId());

                // Redirect to MySessions so they see their new parked car in the list
                return RedirectToAction(nameof(MySessions));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Garages");
            }
        }


        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Pay(int sessionId, decimal amount)
        {
            try
            {
                await _service.PayAsync(sessionId, amount, GetUserId());
                return RedirectToAction(nameof(MySessions));
            }
            catch (Exception ex)
            {
                // Showing why payment failed (e.g. negative amount)
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MySessions));
            }
        }


        // =====================================================
        // DRIVER - EXIT (CLOSE OWN SESSION)
        // =====================================================

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Exit(int sessionId)
        {
            try
            {
                await _service.EndSessionAsync(sessionId, GetUserId());
                return RedirectToAction(nameof(MySessions));
            }
            catch (Exception ex)
            {
                // This catches the "Outstanding balance" exception from Service
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MySessions));
            }
        }

        // =====================================================
        // GARAGE OWNER / ADMIN ZONE
        // =====================================================

        [Authorize(Roles = "GarageOwner,Admin")]
        public async Task<IActionResult> ActiveList()
            {
            // The service now returns IEnumerable<ActiveSessionListViewModel>
            var vm = await _service.GetActiveSessionsForGarageOwnerAsync(GetUserId());

            return View(vm);

            }

        // =====================================================
        // GARAGE OWNER / ADMIN - STOP SESSION
        // =====================================================

        [Authorize(Roles = "GarageOwner,Admin")]
        [HttpPost]
        public async Task<IActionResult> Stop(int sessionId)
        {

            var success = await _service.StopSessionAsync(sessionId, BuildCurrentUser());

            if (!success)
                return Forbid();

            return RedirectToAction(nameof(ActiveList));
        }
    }
}
