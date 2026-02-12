using AutoMapper;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.CarService;
using Microsoft.AspNetCore.Mvc;

namespace CarsInsideGarage.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;
        public CarsController(ICarService carService, IMapper mapper)
        {
            _carService = carService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var carDtos = await _carService.GetAllCarsAsync();
            var viewModels = _mapper.Map<List<CarViewModel>>(carDtos);
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var carDto = await _carService.GetCarByIdAsync(id);

            if (carDto == null)
            {
                // Translation: null (C#) -> 404 (Web Browser)
                return NotFound();
            }

            var viewModel = _mapper.Map<CarViewModel>(carDto);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            if (!ModelState.IsValid) return View(carViewModel);

            try
            {
                // 1. Convert UI model to Data model
                var carDto = _mapper.Map<CarDto>(carViewModel);

                // 2. Save and get the database-generated ID
                int newId = await _carService.AddCarAsync(carDto);

                // 3. REDIRECT: Only send the ID
                return RedirectToAction("AddSuccess", new { carId = newId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("CarPlateNumber", ex.Message);
                return View(carViewModel);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            // We send an empty ViewModel to the view so the form knows what fields to show
            return View(new CarViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> AddSuccess(int carId)
        {
            // 4. GET: Fetch fresh data from the DB using the ID
            var carDto = await _carService.GetCarByIdAsync(carId);

            if (carDto == null) return NotFound();

            // 5. Convert back to ViewModel for the display
            var viewModel = _mapper.Map<CarViewModel>(carDto);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var confirmationVm = await _carService.RemoveCarAsync(id);

            if (confirmationVm == null) return NotFound();

            // Serialize the object to a string to store in TempData
            TempData["DeleteInfo"] = System.Text.Json.JsonSerializer.Serialize(confirmationVm);

            return RedirectToAction("DeleteSuccess");
        }

        public IActionResult DeleteSuccess()
        {
            var data = TempData["DeleteInfo"] as string;
            if (string.IsNullOrEmpty(data)) return RedirectToAction("Index");

            var viewModel = System.Text.Json.JsonSerializer.Deserialize<CarDeleteConfirmationViewModel>(data);
            return View(viewModel);
        }
    }
}
