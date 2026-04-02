using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Services.PricingCalculator;
using CarsInsideGarage.Services.Time;
using Moq;

namespace CarsInsideGarage.Tests
{
    public class PricingCalculatorTests
    {
        private readonly PricingCalculator _calculator;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly DateTime _fixedNow;

        public PricingCalculatorTests()
        {
            _fixedNow = new DateTime(2026, 4, 1, 12, 0, 0);

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _dateTimeProviderMock.Setup(t => t.UtcNow).Returns(_fixedNow);

            _calculator = new PricingCalculator(_dateTimeProviderMock.Object);
        }

        // ================================
        // NORMAL CALCULATION
        // ================================
        [Fact]
        public void CalculateTotal_NormalSession_ReturnsCorrectAmount()
        {
            // Arrange
            var entry = new DateTime(2026, 3, 31, 10, 0, 0);
            var exit = new DateTime(2026, 3, 31, 13, 30, 0); // 3.5 hours

            var session = CreateSession(entry, exit, 2.5m);

            // Act
            var total = _calculator.CalculateTotal(session);

            // Assert
            Assert.Equal(8.75m, total);
        }

        // ================================
        // EXIT <= ENTRY → ZERO
        // ================================
        [Fact]
        public void CalculateTotal_ExitBeforeOrEqualEntry_ReturnsZero()
        {
            // Arrange
            var entry = _fixedNow;
            var exit = _fixedNow.AddHours(-1);

            var session = CreateSession(entry, exit, 5m);

            // Act
            var total = _calculator.CalculateTotal(session);

            // Assert
            Assert.Equal(0m, total);
        }

        // ================================
        // NULL EXIT → USES UtcNow
        // ================================
        [Fact]
        public void CalculateTotal_NullExitTime_UsesUtcNow()
        {
            // Arrange
            var entry = _fixedNow.AddHours(-2);

            var session = CreateSession(entry, null, 3m);

            // Act
            var total = _calculator.CalculateTotal(session);

            // Assert
            Assert.Equal(6m, total); // 2 * 3
        }

        // ================================
        // ROUNDING
        // ================================
        [Fact]
        public void CalculateTotal_RoundsToTwoDecimals()
        {
            // Arrange
            var entry = new DateTime(2026, 3, 31, 10, 0, 0);
            var exit = new DateTime(2026, 3, 31, 11, 15, 0); // 1.25h

            var session = CreateSession(entry, exit, 2.3333m);

            // Act
            var total = _calculator.CalculateTotal(session);

            // Assert
            Assert.Equal(2.92m, total);
        }

        // ================================
        // HELPER (NO REFLECTION)
        // ================================
        private static ParkingSession CreateSession(
            DateTime entry,
            DateTime? exit,
            decimal rate)
        {
            var session = new ParkingSession(
                garageId: 1,
                carId: 1,
                entryTime: entry,
                hourlyRate: rate,
                dailyRate: 0m,
                monthlyRate: 0m
            );

            // We still need this ONE controlled reflection
            // because ExitTime is private and only set via Close()
            if (exit.HasValue)
            {
                typeof(ParkingSession)
                    .GetProperty("ExitTime", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)!
                    .SetValue(session, exit);
            }

            return session;
        }
    }
}