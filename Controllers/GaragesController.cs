using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Garage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Controllers
{
    public class GaragesController : Controller
    {
        private readonly IGarageService _garageService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public GaragesController(IGarageService garageService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
             _garageService = garageService;
            _mapper = mapper;
            _userManager = userManager;
        }

        private CurrentUser BuildCurrentUser()
        {
            return new CurrentUser
            {
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                IsAdmin = User.IsInRole("Admin"),
                IsOwner = User.IsInRole("GarageOwner"),
                IsDriver = User.IsInRole("Driver")
            };
        }

        // =============================
        // LIST
        // =============================

        public async Task<IActionResult> Index()
        {
            var user = BuildCurrentUser();

            var garages = await _garageService.GetAllAsync(user);

            TempData["Step"] = 1; // Share geolocation

            return View(garages);
        }

        // =============================
        // DETAILS
        // =============================

        public async Task<IActionResult> Details(int id)
        {
            var user = BuildCurrentUser();

            var garageDto = await _garageService.GetDetailsViewModelAsync(id, user);
            
            if (garageDto == null) 
                return NotFound();

            var viewModel = _mapper.Map<GarageDetailsViewModel>(garageDto);

            TempData["Step"] = 2; // Select a parking lot

            return View(viewModel);
        }

        // =============================
        // CREATE - GET
        // =============================

        [HttpGet]
		[Authorize(Roles = "GarageOwner")]
        public IActionResult Create()
        {
            return View(new GarageCreateViewModel());
        }

        // =============================
        // CREATE - POST
        // =============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Create(GarageCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            var user = BuildCurrentUser();

            if (string.IsNullOrEmpty(user.UserId))
                return Unauthorized();
          
            var dto = new GarageCreateDto
            {
                Name = model.Name,
                Capacity = model.Capacity,
                Area = model.SelectedArea.ToString(),
                Lat = model.Lat,
                Lng = model.Lng,
                HourlyRate = model.HourlyRate,
                DailyRate = model.DailyRate,
                MonthlyRate = model.MonthlyRate,
                Rules = model.Rules == null
                    ? new List<PricingRuleCreateDto>()
                    : _mapper.Map<List<PricingRuleCreateDto>>(model.Rules)
			};

            var createdGarageId = await _garageService.CreateAsync(dto, user);

            return RedirectToAction(nameof(Details), new { id = createdGarageId });
        }

        // =============================
        // DELETE
        // =============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,GarageOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = BuildCurrentUser();

            if (string.IsNullOrEmpty(user.UserId)) 
                return Unauthorized();

            try
            {
                var deleteVm = await _garageService.DeleteGarageAsync(id, user);

                return View("DeleteSuccess", deleteVm);
            }
            catch 
            {
                return NotFound();
            }
        }

        // =============================
        // REVENUE REPORT
        // =============================

        [HttpGet]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> RevenueReport()
        {
            var user = BuildCurrentUser();

            if (string.IsNullOrEmpty(user.UserId)) 
                return Unauthorized();

            var dtos = await _garageService.GetRevenueReportAsync(user);
            var vm = _mapper.Map<IEnumerable<RevenueReportViewModel>>(dtos);

            return View(vm);
        }

        // =============================
        // NEARBY GARAGE SEARCH
        // =============================

        [HttpGet]
        public async Task<IActionResult> GetNearbyGarages(double lat, double lng, int count = 5)
        {
            var garages = await _garageService.GetNearestManyAsync(lat, lng, count);

            var dtos = garages.Select(g => new GarageNearbyDto
                {
                Id = g.Id,
                Name = g.Name,
                Latitude = g.Location.Latitude,
                Longitude = g.Location.Longitude,
                FreeSpots = g.Capacity - (g.Sessions?.Count(s => s.ExitTime == null) ?? 0),
                Distance = 0
                }).ToList();

            var viewModels = _mapper.Map<IEnumerable<GarageNearbyViewModel>>(dtos);


            // NOTE: This returns a View intentionally.
            // JSON responses will be handled by the API layer in the next phase.

            return View(viewModels);
            }

        // =============================
        // PRICING POLICY MANAGEMENT
        // =============================

        [HttpGet]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> UpdatePricing(int id)
        {
            var user = BuildCurrentUser();

            if (string.IsNullOrEmpty(user.UserId))
                return Unauthorized();

            var garage = await _garageService.GetDetailsViewModelAsync(id, user);

            if (garage == null)
                return NotFound();

            // Map to PricingUpdateViewModel
            var vm = new PricingUpdateViewModel
            {
                GarageId = garage.Id,
                HourlyRate = garage.HourlyRate,
                DailyRate = garage.DailyRate,
                MonthlyRate = garage.MonthlyRate,
                Rules = new List<PricingRuleDetailsViewModel>() // start empty for now
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> UpdatePricing(PricingUpdateViewModel model)
        {
            var user = BuildCurrentUser();

            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<PricingUpdateDto>(model);

            await _garageService.UpdatePricingAsync(model.GarageId, dto, user);

            return RedirectToAction(nameof(Details), new { id = model.GarageId });
        }


    }
}
