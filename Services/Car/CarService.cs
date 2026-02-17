using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.CarService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CarsInsideGarage.Services.Car
{
    public class CarService : ICarService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GeometryFactory _geometryFactory =
            NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        public CarService(GarageDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // ================================
        // LIST
        // ================================

        public async Task<IEnumerable<CarDto>> GetAllCarsAsync()
        {
            // 1. Identify logged-in user and their role
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = _userManager.GetUserId(user);

            // In case of user not logged in (no user), return an empty list 
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Enumerable.Empty<CarDto>();
            }

            // 2. Start the query
            var query = _context.Cars.AsQueryable();

            // 3. Filter: If not an Admin, only show own cars
            if (user != null && !user.IsInRole("Admin"))
            {
                query = query.Where(c => c.UserId == userId);
            }

            // 4. Project directly to DTO and return the result
            return await query
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ================================
        // SEARCH
        // ================================

        public async Task<CarDto> GetCarByIdAsync(int id)
        {
            // Fetches the car where the ID matches. Returns null if not found.
            var carEntity = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carEntity == null)
            {
                // Could return null, or handle this with an exception
                return null; // "I searched, but found nothing."
            }

            return _mapper.Map<CarDto>(carEntity);
        }


        // ================================
        // CREATE
        // ================================

        public async Task<int> AddCarAsync(CarDto carDto)
        {
            // 1. Clean up the plate number
            carDto.CarPlateNumber = carDto.CarPlateNumber.Trim().Replace(" ", "").ToUpper();

            // 2. Identify logged-in user and their role
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var userId = _userManager.GetUserId(userClaims);

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("You must be logged in to register a car.");
            }

            // 3. Check for duplicates
            var exists = await _context.Cars
                .AnyAsync(c => c.CarPlateNumber == carDto.CarPlateNumber);

            if (exists) throw new Exception("A car with this license plate already exists.");

            var carEntity = _mapper.Map<CarsInsideGarage.Data.Entities.Car>(carDto);

            carEntity.UserId = userId;

            _context.Cars.Add(carEntity);
            await _context.SaveChangesAsync();

            return carEntity.Id;
        }


        // ================================
        // DELETE - ToDo!!!
        // ================================

        public async Task<CarDeleteConfirmationViewModel> RemoveCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return null;

            // Capture the data BEFORE it's deleted
            var vm = new CarDeleteConfirmationViewModel
            {
                CarPlateNumber = car.CarPlateNumber,
                ExitTime = DateTime.Now
            };

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return vm; // Returning the "Ghost" of the deleted Car to the Controller
        }
    }
}
