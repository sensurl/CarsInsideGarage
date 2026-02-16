using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.GarageSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarsInsideGarage.Controllers
{
    public class ParkingSessionsController : Controller
    {
        private readonly IParkingSessionService _service;
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;

        public ParkingSessionsController(IParkingSessionService service, GarageDbContext dbContext, IMapper mapper)
        {
            _service = service;
            _context = dbContext;
            _mapper = mapper;
        }

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Active()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (car == null)
                return View("NoActiveSession");

            var session = await _service.GetActiveSessionByCarAsync(car.Id);

            if (session == null)
                return View("NoActiveSession");

            return View(session);
        }


        public async Task<IActionResult> ActiveList()
        {
            var sessions = await _context.ParkingSessions
                .Include(s => s.Car)
                .Include(s => s.Garage)
                .Where(s => s.ExitTime == null)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<ActiveSessionListDto>>(sessions);

            var vm = _mapper.Map<IEnumerable<ActiveSessionListViewModel>>(dtos);

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Pay(int sessionId, decimal amount)
        {
            try
            {
                await _service.PayAsync(sessionId, amount);
                return RedirectToAction("Active");
            }
            catch (Exception ex)
            {
                // Showing why payment failed (e.g. negative amount)
                TempData["Error"] = ex.Message;

                return RedirectToAction("Active");
            }
        }

        [Authorize(Roles = "Driver")]
        [HttpGet]
        public async Task<IActionResult> Start(int garageId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (car == null)
                return RedirectToAction("Index", "Cars");

            try
            {
                await _service.StartSessionAsync(garageId, car.Id);
                return RedirectToAction("Active");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Garages");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Exit(int sessionId)
        {
            int carId = await _service.GetCarIdBySessionId(sessionId);

            try
            {
                await _service.EndSessionAsync(sessionId);

                return RedirectToAction("Active", new { carId });
            }
            catch (Exception ex)
            {
                // This catches the "Outstanding balance" exception from Service
                TempData["Error"] = ex.Message;
                return RedirectToAction("Active", new { carId });
            }
        }
    }
}
