using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Fee;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Controllers
{
    public class GaragesController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IFeeService _feeService;
        private readonly IGarageService _garageService;
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;

        public GaragesController(IGarageService garageService, ILocationService locationService, IFeeService feeService, GarageDbContext context, IMapper mapper)
        {
            _locationService = locationService;
            _feeService = feeService;
            _garageService = garageService;
            _context = context;
            _mapper = mapper;
        }

        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllAsync();
            return View(garages);
        }



        public async Task<IActionResult> Details(int id, bool isOwner = true)
        {
            var garageDto = await _garageService.GetDetailsViewModelAsync(id, isOwner);
            if (garageDto == null) return NotFound();

            var viewModel = _mapper.Map<GarageDetailsViewModel>(garageDto);

            //ToDo -- isOwner true must be removed; actually Details is the only place the Driver can register their car!  so this must be Public!

            // Pick the first car in the database to simulate "The Current User"
            /*
            var car = await _context.Cars.FirstOrDefaultAsync();
            if (car != null)
            {
                ViewBag.CurrentCarId = car.Id;
            }
*/
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new GarageCreateViewModel();
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Create(GarageCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
          
            var dto = new GarageCreateDto
            {
                Name = model.Name,
                Capacity = model.Capacity,
                Area = model.SelectedArea.ToString(),
                ParkingCoordinates = model.ParkingCoordinates,
                ParkingFeeId = model.ParkingFeeId
            };

            var createdGarageId = await _garageService.CreateAsync(dto, userId);

            return RedirectToAction(nameof(Details), new { id = createdGarageId });
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleteVm = await _garageService.DeleteGarageAsync(id);

                return View("DeleteSuccess", deleteVm);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        private async Task PopulateDropdownsAsync(GarageCreateViewModel vm)
        {

            var fees = await _feeService.GetAllAsync();

            vm.Fees = fees.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = $"H: {f.HourlyRate} / D: {f.DailyRate} / M: {f.MonthlyRate}"
            });
        }

        // [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> RevenueReport()
        {
            var garages = await _context.Garages
                .Include(g => g.Sessions)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<RevenueReportDto>>(garages);
            var vm = _mapper.Map<IEnumerable<RevenueReportViewModel>>(dtos);

            return View(vm);
        }


    }
}