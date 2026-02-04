using CarsInsideGarage.Data;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.Garage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarsInsideGarage.Controllers
{
    public class GaragesController : Controller
    {
        private readonly IGarageService _garageService;
        private readonly GarageDbContext _context;

        public GaragesController(IGarageService garageService, GarageDbContext context)
        {
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
        public IActionResult Create()
        {
            var vm = new GarageCreateViewModel
            {
                Locations = _context.Locations
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = $"{l.Area} - {l.AddressCoordinates}"
                    }),
                Fees = _context.GarageFees
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = $"{f.HourlyRate} per hour"
                    })
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GarageCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Locations = _context.Locations
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Area.ToString()
                    });

                vm.Fees = _context.GarageFees
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.HourlyRate.ToString()
                    });

                return View(vm);
            }

            await _garageService.CreateAsync(
                vm.Name,
                vm.Capacity,
                vm.LocationId,
                vm.GarageFeeId);

            return RedirectToAction(nameof(Index));
        }
    }
}
