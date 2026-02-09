using CarsInsideGarage.Data;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Fee;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarsInsideGarage.Controllers
{
    public class GaragesController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IFeeService _feeService;
        private readonly IGarageService _garageService;
        private readonly GarageDbContext _context;

        public GaragesController(IGarageService garageService, ILocationService locationService, IFeeService feeService, GarageDbContext context)
        {
            _locationService = locationService;
            _feeService = feeService;
            _garageService = garageService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllAsync();
            return View(garages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var garage = await _garageService.GetGarageDetailsAsync(id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new GarageCreateViewModel();
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(GarageCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            try
            {
                await _garageService.CreateAsync(
                    vm.Name,
                    vm.Capacity,
                    vm.SelectedArea,
                    vm.AddressCoordinates,
                    vm.SelectedFeeId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception) // You can be more specific here (e.g., DbUpdateException)
            {
                ModelState.AddModelError("AddressCoordinates", "These coordinates are already assigned to another garage.");
                await PopulateDropdownsAsync(vm);
                return View(vm);
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
    }
}