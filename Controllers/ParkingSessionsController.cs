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
        public async Task<IActionResult> Active()
        {
            var session = await _service
            .GetActiveSessionForDriverAsync(GetUserId());

            if (session == null)
                return View("NoActiveSession");

            return View(session);
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public async Task<IActionResult> Start(int garageId)
        {
            try
            {
                await _service.StartSessionAsync(garageId, GetUserId());
                return RedirectToAction(nameof(Active));
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
                return RedirectToAction(nameof(Active));
            }
            catch (Exception ex)
            {
                // Showing why payment failed (e.g. negative amount)
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Active));
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
                return RedirectToAction(nameof(Active));
            }
            catch (Exception ex)
            {
                // This catches the "Outstanding balance" exception from Service
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Active));
            }
        }

        // =====================================================
        // GARAGE OWNER / ADMIN ZONE
        // =====================================================

        [Authorize(Roles = "GarageOwner,Admin")]
        public async Task<IActionResult> ActiveList()
        {
            var sessions = await _service.GetActiveSessionsForGarageOwnerAsync(GetUserId());

            var dtos = _mapper.Map<IEnumerable<ActiveSessionListDto>>(sessions);

            var vm = _mapper.Map<IEnumerable<ActiveSessionListViewModel>>(dtos);

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
