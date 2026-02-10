using CarsInsideGarage.Data;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Fee;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;

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

        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllAsync();
            return View(garages);
        }

        

        public async Task<IActionResult> Details(int id)
        {
            // TEMPORARY: until Identity exists
            bool isOwner = true; // change to false to simulate driver

            var model = await _garageService
                .GetDetailsViewModelAsync(id, isOwner);

            if (model == null)
                return NotFound();

            return View(model);
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

                return View("CreateSuccess", vm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("AddressCoordinates", "These coordinates are already assigned to another garage.");
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }
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
    }
}