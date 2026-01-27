using AutoMapper;
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
    }
}
