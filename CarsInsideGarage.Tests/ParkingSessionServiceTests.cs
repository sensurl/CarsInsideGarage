using AutoMapper;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.GarageSession;
using CarsInsideGarage.Services.PricingCalculator;
using CarsInsideGarage.Services.Time;
using Moq;

namespace CarsInsideGarage.Tests
{
    public class ParkingSessionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly IPricingCalculator _pricingCalculator;
        private readonly ParkingSessionService _service;

        private readonly DateTime _fixedNow;

        public ParkingSessionServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _fixedNow = new DateTime(2026, 4, 1, 12, 0, 0);

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _dateTimeProviderMock.Setup(t => t.UtcNow).Returns(_fixedNow);

            _pricingCalculator = new PricingCalculator(_dateTimeProviderMock.Object);

            _service = new ParkingSessionService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _pricingCalculator,
                _dateTimeProviderMock.Object
            );
        }

        // ================================
        // TEST: Get Active Session Details
        // ================================
        [Fact]
        public async Task GetActiveSessionDetailsAsync_ValidSession_ReturnsViewModel()
        {
            // Arrange
            var entryTime = _fixedNow.AddHours(-2);

            var session = CreateSession(entryTime, 2m);

            var car = new Car { Id = 1, UserId = "user1" };

            _unitOfWorkMock.Setup(u => u.Cars.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(session);

            _mapperMock.Setup(m => m.Map<SessionDto>(session))
                .Returns(new SessionDto());

            _mapperMock.Setup(m => m.Map<SessionActiveViewModel>(It.IsAny<SessionDto>()))
                .Returns(new SessionActiveViewModel());

            // Act
            var result = await _service.GetActiveSessionDetailsAsync(session.Id, "user1");

            // Assert
            Assert.NotNull(result);

            // 2 hours * 2 rate = 4
            Assert.Equal(4m, result.AccruedAmount);
        }

        // ================================
        // TEST: Pay
        // ================================
        [Fact]
        public async Task PayAsync_ValidPayment_UpdatesSession()
        {
            // Arrange
            var entryTime = _fixedNow.AddHours(-2);
            var session = CreateSession(entryTime, 2m);

            var car = new Car { Id = 1, UserId = "user1" };

            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(session.Id))
                .ReturnsAsync(session);

            _unitOfWorkMock.Setup(u => u.Cars.GetByIdAsync(session.CarId))
                .ReturnsAsync(car);

            _unitOfWorkMock.Setup(u => u.Sessions.Update(session));
            _unitOfWorkMock.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            await _service.PayAsync(session.Id, 5m, "user1");

            // Assert
            Assert.Equal(5m, session.AmountPaid);
            
        }

        // ================================
        // TEST: End Session - Not Paid
        // ================================
        [Fact]
        public async Task EndSessionAsync_AmountPaidLessThanDue_ThrowsException()
        {
            // Arrange
            var entryTime = _fixedNow.AddHours(-2);
            var session = CreateSession(entryTime, 2m);

            session.RegisterPayment(1m); // due = 4, paid = 1

            var car = new Car { Id = 1, UserId = "user1" };

            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(session.Id))
                .ReturnsAsync(session);

            _unitOfWorkMock.Setup(u => u.Cars.GetByIdAsync(session.CarId))
                .ReturnsAsync(car);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _service.EndSessionAsync(session.Id, "user1"));
        }

        // ================================
        // HELPER: Clean Session Creation
        // ================================
        private static ParkingSession CreateSession(
            DateTime entryTime,
            decimal hourlyRate)
        {
            return new ParkingSession(
                garageId: 1,
                carId: 1,
                entryTime: entryTime,
                hourlyRate: hourlyRate,
                dailyRate: 0m,
                monthlyRate: 0m
            );
        }
    }
}