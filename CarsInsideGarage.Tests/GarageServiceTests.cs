using AutoMapper;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.Time;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace CarsInsideGarage.Tests
{
    public class GarageServiceTests
    {
        private readonly Mock<IGarageRepository> _garageRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly GarageService _garageService;

        private readonly DateTime _fixedNow;

        public GarageServiceTests()
        {
            _garageRepoMock = new Mock<IGarageRepository>();

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.Garages).Returns(_garageRepoMock.Object);

            _mapperMock = new Mock<IMapper>();

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fixedNow = new DateTime(2026, 4, 1, 12, 0, 0);
            _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(_fixedNow);

            _garageService = new GarageService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _dateTimeProviderMock.Object
            );
        }

        // -------------------------
        // GET ALL
        // -------------------------
        [Fact]
        public async Task GetAllAsync_Admin_ReturnsAll()
        {
            var garages = new List<Garage>
            {
                CreateGarage("G1", "U1"),
                CreateGarage("G2", "U2")
            };

            _garageRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(garages);

            _mapperMock.Setup(m => m.Map<List<GarageListDto>>(It.IsAny<IEnumerable<Garage>>()))
                .Returns((IEnumerable<Garage> g) =>
                    g.Select(x => new GarageListDto { Name = x.Name }).ToList());

            var user = new CurrentUser { IsAdmin = true };

            var result = await _garageService.GetAllAsync(user);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_NonAdmin_ReturnsOwnOnly()
        {
            var garages = new List<Garage>
            {
                CreateGarage("G1", "U1"),
                CreateGarage("G2", "U2")
            };

            _garageRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(garages);

            _mapperMock.Setup(m => m.Map<List<GarageListDto>>(It.IsAny<IEnumerable<Garage>>()))
                .Returns((IEnumerable<Garage> g) =>
                    g.Select(x => new GarageListDto { Name = x.Name }).ToList());

            var user = new CurrentUser { UserId = "U1" };

            var result = await _garageService.GetAllAsync(user);

            Assert.Single(result);
            Assert.Equal("G1", result.First().Name);
        }

        // -------------------------
        // CREATE
        // -------------------------
        [Fact]
        public async Task CreateAsync_ReturnsId()
        {
            _garageRepoMock.Setup(r => r.AddAsync(It.IsAny<Garage>()))
                .Callback<Garage>(g => g.Id = 123)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1); // ✅ FIXED

            var dto = new GarageCreateDto
            {
                Name = "New Garage",
                Capacity = 10,
                Area = "Center",
                Lat = 1,
                Lng = 2,
                HourlyRate = 5,
                DailyRate = 20,
                MonthlyRate = 200
            };

            var result = await _garageService.CreateAsync(dto, new CurrentUser { UserId = "U1" });

            Assert.Equal(123, result);
        }

        // -------------------------
        // DELETE
        // -------------------------
        [Fact]
        public async Task DeleteGarageAsync_Admin_HardDelete()
        {
            var garage = CreateGarage("G1", "U1");

            _garageRepoMock.Setup(r => r.GetGarageWithDetailsAsync(garage.Id))
                .ReturnsAsync(garage);

            _unitOfWorkMock.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1); // ✅ FIXED

            var result = await _garageService.DeleteGarageAsync(garage.Id, new CurrentUser { IsAdmin = true });

            Assert.Equal("G1", result.Name);
            _garageRepoMock.Verify(r => r.Remove(garage), Times.Once);
        }

        // -------------------------
        // UPDATE PRICING
        // -------------------------
        [Fact]
        public async Task UpdatePricingAsync_UpdatesPolicy()
        {
            var garage = CreateGarage("G1", "U1");

            _garageRepoMock.Setup(r => r.GetGarageWithDetailsAsync(garage.Id))
                .ReturnsAsync(garage);

            _unitOfWorkMock.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1); // ✅ FIXED

            var dto = new PricingUpdateDto
            {
                HourlyRate = 10,
                DailyRate = 50,
                MonthlyRate = 500
            };

            await _garageService.UpdatePricingAsync(garage.Id, dto, new CurrentUser { IsOwner = true, UserId = "U1" });

            Assert.Equal(10, garage.PricingPolicy.HourlyRate);
        }

        // -------------------------
        // REVENUE
        // -------------------------
        [Fact]
        public async Task GetRevenueReportAsync_ReturnsData()
        {
            var garage = CreateGarage("G1", "U1");
            garage.Sessions.Add(CreateSession(_fixedNow.AddHours(-2), null, 2m));

            _garageRepoMock.Setup(r => r.WhereAsync(It.IsAny<Expression<Func<Garage, bool>>>()))
                .ReturnsAsync(new List<Garage> { garage });

            _garageRepoMock.Setup(r => r.GetGarageWithDetailsAsync(garage.Id))
                .ReturnsAsync(garage);

            var result = await _garageService.GetRevenueReportAsync(
                new CurrentUser { IsOwner = true, UserId = "U1" });

            Assert.Single(result);
            Assert.Equal(4m, result.First().TotalRevenue); // 2h * 2
        }

        // -------------------------
        // HELPERS
        // -------------------------
        private static Garage CreateGarage(string name, string userId)
        {
            var location = new Location(Area.Center, 1, 2);
            var policy = new PricingPolicy(5, 20, 200);
            return new Garage(name, 5, location, policy, userId);
        }

        private static ParkingSession CreateSession(DateTime entry, DateTime? exit, decimal rate)
        {
            var session = (ParkingSession)Activator.CreateInstance(typeof(ParkingSession), nonPublic: true)!;

            typeof(ParkingSession).GetProperty("EntryTime")!
                .SetValue(session, entry);

            typeof(ParkingSession).GetProperty("ExitTime")!
                .SetValue(session, exit);

            typeof(ParkingSession).GetProperty("HourlyRate")!
                .SetValue(session, rate);

            return session;
        }
    }
}