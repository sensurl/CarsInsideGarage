using Microsoft.AspNetCore.Mvc;
using CarsInsideGarage.Services.Fee;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Models.DTOs;
using AutoMapper;

namespace CarsInsideGarage.Controllers
{
    public class FeesController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly IMapper _mapper;

        public FeesController(IFeeService feeService, IMapper mapper)
        {
            _feeService = feeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var feeDtos = await _feeService.GetAllAsync();

            var viewModel = _mapper.Map<IEnumerable<FeeCreateViewModel>>(feeDtos);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new FeeCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeeCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<FeeDto>(vm);
            await _feeService.CreateAsync(dto);

            return View("CreateSuccess", vm);
        }
    }
}