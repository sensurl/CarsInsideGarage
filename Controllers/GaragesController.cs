using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Controllers
{
    public class GaragesController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IGarageService _garageService;
		private readonly IGarageLocationService _garageLocationService;
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public GaragesController(IGarageService garageService, ILocationService locationService, IGarageLocationService garageLocationService, GarageDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
             _garageService = garageService;
            _locationService = locationService;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
			_garageLocationService = garageLocationService;
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
                ParkingCoordinates = model.ParkingCoordinates,
				
				// 🔥 Pricing now comes directly
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
        [Authorize(Roles = "Admin")]
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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> RevenueReport()
        {
            var garages = await _context.Garages
                .Include(g => g.Sessions)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<RevenueReportDto>>(garages);
            var vm = _mapper.Map<IEnumerable<RevenueReportViewModel>>(dtos);

            return View(vm);
        }

        // =============================
        // NEARBY GARAGE SEARCH
        // =============================

       
               
        [HttpGet]
        public async Task<IActionResult> GetNearbyGarages(double lat, double lng, int count = 5)
        {
            var garages = await _garageLocationService.GetNearestManyAsync(lat, lng, count);

            var dtos = garages.Select(g => new GarageNearbyDto
                {
                Id = g.Id,
                Name = g.Name,
                Latitude = g.Location!.ParkingCoordinates!.Y,
                Longitude = g.Location!.ParkingCoordinates!.X,
                FreeSpots = g.Capacity - (g.Sessions?.Count(s => s.ExitTime == null) ?? 0),
                Distance = 0
                }).ToList();

            var viewModels = _mapper.Map<IEnumerable<GarageNearbyViewModel>>(dtos);

            return View(viewModels);
            }
       

        }
    }
