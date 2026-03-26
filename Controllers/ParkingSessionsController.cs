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

        // MySessions.cshtml is the user's landing page. It shows a list.
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> MySessions()
        {
            var sessions = await _service.GetActiveSessionsForDriverAsync(GetUserId());

            if (!sessions.Any())
                return View("NoActiveSession");

            return View(sessions);
        }

        // Active.cshtml shows after the Driver clicks "Details & Payment" in the MysSessions.cshtml. This page focuses strictly on the financial transaction and the physical exit. 
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Active(int? id)
        {
            // If no session ID is provided, we dont know which Car they mean. We send them back to the list so they can pick a Car.
            if (id == null) 
                return RedirectToAction(nameof(MySessions));

            var session = await _service.GetActiveSessionDetailsAsync(id.Value, GetUserId());

            // If the session doesn't exist or is already closed, send them back
            if (session == null)
            {
                TempData["Error"] = "This session is no longer active.";
                return RedirectToAction(nameof(MySessions));
            }

            return View(session);
        }


        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Start(int garageId, int? carId)
        {
            try
            {
                // If carId is null, the user clicks "Park" from the Garage list
                if (carId == null)
                {
                    // Check how many Cars the user has. 
                   
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

                ViewBag.Step = 3; // Start parking session; Car Parked

                // Driver only: A "Dashboard" of the user's parked cars. This is the New List View.
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

                ViewBag.Step = 4; // pay & exit

                // SUCCESS → redirect to success page
                return RedirectToAction(nameof(PaymentSuccess), new { sessionId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MySessions));
            }

        }

        [Authorize(Roles = "Driver")]
        public IActionResult PaymentSuccess(int sessionId)
        {
            return View(model: sessionId);
        }



        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Exit(int sessionId)
        {
            try
            {
                await _service.EndSessionAsync(sessionId, GetUserId());
                return RedirectToAction(nameof(ExitSuccess), new { sessionId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MySessions));
            }

        }

        [Authorize(Roles = "Driver")]
        public IActionResult ExitSuccess(int sessionId)
        {
            return View(model: sessionId);
        }


        // =====================================================
        // GARAGE OWNER / ADMIN ZONE
        // =====================================================

        // ActiveList.cshtml is the equivalent of the MySessions.cshtml for the Garage Owner. It shows all active sessions in the owner's garage(s). This is the "Occupancy" monitoring page.
        [Authorize(Roles = "GarageOwner,Admin")]
        public async Task<IActionResult> ActiveList()
            {
            
            var vm = await _service.GetActiveSessionsForGarageOwnerAsync(GetUserId());

            return View(vm);

            }


        [Authorize(Roles = "GarageOwner,Admin")]
        [HttpPost]
        public async Task<IActionResult> Stop(int sessionId)
        {

            var success = await _service.StopSessionAsync(sessionId, BuildCurrentUser());

            if (!success)
                return Forbid();

            // Owner/Admin: High-level "Occupancy" monitoring. No personal "Pay" or "Exit" buttons.
            return RedirectToAction(nameof(ActiveList));
        }
    }
}
