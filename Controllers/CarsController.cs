using AutoMapper;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.CarService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarsInsideGarage.Controllers
{
   
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarsController(ICarService carService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _carService = carService;
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


        // ================================
        // LIST ALL - VISIBLE TO ALL ROLES
        // ================================

        public async Task<IActionResult> Index()
        {
           var user = BuildCurrentUser();

            var carDtos = await _carService.GetAllCarsAsync(user);
            
            var viewModels = _mapper.Map<List<CarViewModel>>(carDtos);
            
            return View(viewModels);
        }

        // ================================
        // DETAILS PER UNIT - DRIVER ONLY
        // ================================

        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Details(int id)
        {
            var user = BuildCurrentUser();

            var carDto = await _carService.GetCarByIdAsync(id, user);

            if (carDto == null)
                return NotFound(); // Translation: null (C#) -> 404 (Web Browser)

            var viewModel = _mapper.Map<CarViewModel>(carDto);

            return View(viewModel);
        }

        // ================================
        // CREATE - ALL ROLES
        // ================================

        [HttpGet]
        public IActionResult Create()
            {
            // We send an empty ViewModel to the view so the form knows what fields to show
            return View(new CarViewModel());
            }

        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            if (!ModelState.IsValid) return View(carViewModel);

            var user = BuildCurrentUser();

            try
            {
                // 1. Convert UI model to Data model
                var carDto = _mapper.Map<CarDto>(carViewModel);

                // 2. Save and get the database-generated ID
                int newId = await _carService.AddCarAsync(carDto, user!);

                TempData["Step"] = 0; // Register Vehicle

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
        public async Task<IActionResult> AddSuccess(int carId)
        {
            var user = BuildCurrentUser();

            // 4. GET: Fetch fresh data from the DB using the ID
            var carDto = await _carService.GetCarByIdAsync(carId, user);

            if (carDto == null) 
                return NotFound();

            // 5. Convert back to ViewModel for the display
            var viewModel = _mapper.Map<CarViewModel>(carDto);

            return View(viewModel);
        }


        // ================================
        // DELETE
        // ================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = BuildCurrentUser();

            var confirmationVm = await _carService.RemoveCarAsync(id, user);

            if (confirmationVm == null) 
                return NotFound();

            // Serialize the object to a string to store in TempData
            TempData["DeleteInfo"] = System.Text.Json.JsonSerializer.Serialize(confirmationVm);

            return RedirectToAction("DeleteSuccess");
        }

        public IActionResult DeleteSuccess()
        {
            var data = TempData["DeleteInfo"] as string;
            if (string.IsNullOrEmpty(data)) 
                return RedirectToAction("Index");

            var viewModel = System.Text.Json.JsonSerializer
                .Deserialize<CarDeleteConfirmationViewModel>(data);
            return View(viewModel);
        }


        // ================================
        // RESTORE - ADMIN ONLY
        // ================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(int id)
        {
            var user = BuildCurrentUser();

            var restored = await _carService.RestoreCarAsync(id, user);

            if (!restored)
                return NotFound();

            return RedirectToAction("Index");
        }

    }
}
