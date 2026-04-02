using AutoMapper;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Services.Car;
using CarsInsideGarage.Services.Time;
using Moq;

namespace CarsInsideGarage.Tests
{
    public class CarServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly CarService _carService;

        private readonly DateTime _fixedNow;

        public CarServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            // Fixed time for deterministic tests
            _fixedNow = new DateTime(2026, 4, 1, 12, 0, 0);
            _dateTimeProviderMock.Setup(t => t.UtcNow).Returns(_fixedNow);

            _carService = new CarService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _dateTimeProviderMock.Object
            );
        }

        // ================================
        // GET ALL - ADMIN
        // ================================
        [Fact]
        public async Task GetAllCarsAsync_AdminUser_ReturnsAllCars()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" },
                new Car { Id = 2, CarPlateNumber = "BBB222", UserId = "U2" }
            };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cars);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);

            var user = new CurrentUser { IsAdmin = true, UserId = "U1" };

            _mapperMock.Setup(m => m.Map<List<CarDto>>(cars))
                .Returns(cars.Select(c => new CarDto
                {
                    Id = c.Id,
                    CarPlateNumber = c.CarPlateNumber
                }).ToList());

            // Act
            var result = await _carService.GetAllCarsAsync(user);

            // Assert
            Assert.Equal(2, result.Count());
        }

        // ================================
        // GET ALL - DRIVER
        // ================================
        [Fact]
        public async Task GetAllCarsAsync_Driver_ReturnsOnlyOwnCars()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" },
                new Car { Id = 2, CarPlateNumber = "BBB222", UserId = "U2" }
            };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cars);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);

            var user = new CurrentUser { IsDriver = true, UserId = "U1" };

            var filteredCars = cars.Where(c => c.UserId == "U1").ToList();

            _mapperMock.Setup(m => m.Map<List<CarDto>>(filteredCars))
                .Returns(filteredCars.Select(c => new CarDto
                {
                    Id = c.Id,
                    CarPlateNumber = c.CarPlateNumber
                }).ToList());

            // Act
            var result = await _carService.GetAllCarsAsync(user);

            // Assert
            Assert.Single(result);
            Assert.Equal("AAA111", result.First().CarPlateNumber);
        }

        // ================================
        // GET BY ID - ADMIN
        // ================================
        [Fact]
        public async Task GetCarByIdAsync_AdminCanGetAnyCar()
        {
            // Arrange
            var car = new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);

            var user = new CurrentUser { IsAdmin = true, UserId = "U2" };

            _mapperMock.Setup(m => m.Map<CarDto>(car))
                .Returns(new CarDto
                {
                    Id = car.Id,
                    CarPlateNumber = car.CarPlateNumber
                });

            // Act
            var result = await _carService.GetCarByIdAsync(1, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA111", result!.CarPlateNumber);
        }

        // ================================
        // GET BY ID - UNAUTHORIZED
        // ================================
        [Fact]
        public async Task GetCarByIdAsync_NonAdminCannotGetOthersCar()
        {
            // Arrange
            var car = new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);

            var user = new CurrentUser { IsDriver = true, UserId = "U2" };

            // Act
            var result = await _carService.GetCarByIdAsync(1, user);

            // Assert
            Assert.Null(result);
        }

        // ================================
        // DELETE - SOFT DELETE (DRIVER)
        // ================================
        [Fact]
        public async Task RemoveCarAsync_DriverSoftDeletesOwnCar()
        {
            // Arrange
            var car = new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var user = new CurrentUser { IsDriver = true, UserId = "U1" };

            // Act
            var result = await _carService.RemoveCarAsync(1, user);

            // Assert
            Assert.NotNull(result);
            Assert.True(car.IsDeleted);
            Assert.Equal(_fixedNow, car.DeletedAt);
        }

        // ================================
        // DELETE - HARD DELETE (ADMIN)
        // ================================
        [Fact]
        public async Task RemoveCarAsync_AdminHardDeletesCar()
        {
            // Arrange
            var car = new Car { Id = 1, CarPlateNumber = "AAA111", UserId = "U1" };

            var mockCarsRepo = new Mock<IRepository<Car>>();
            mockCarsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Cars).Returns(mockCarsRepo.Object);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var user = new CurrentUser { IsAdmin = true, UserId = "admin" };

            // Act
            var result = await _carService.RemoveCarAsync(1, user);

            // Assert
            Assert.NotNull(result);

            mockCarsRepo.Verify(r => r.Remove(car), Times.Once);
        }
    }
}