using CarsInsideGarage.Data.Enums;

namespace CarsInsideGarage.Data.Entities
{
    public class ParkingSession
    {
        public int Id { get; private set; }

        public int GarageId { get; private set; }
        public Garage Garage { get; private set; } = null!;

        public int CarId { get; private set; }
        public Car Car { get; private set; } = null!;

        public DateTime EntryTime { get; private set; }
        public DateTime? ExitTime { get; private set; }

        public ParkingSessionStatus Status { get; private set; }

        public decimal HourlyRate { get; private set; }
        public decimal DailyRate { get; private set; }
        public decimal MonthlyRate { get; private set; }

        public decimal AmountPaid { get; private set; }
        public bool IsCleared { get; private set; }

        private ParkingSession() { }

        public ParkingSession(
            int garageId,
            int carId,
            decimal hourlyRate,
            decimal dailyRate,
            decimal monthlyRate)
        {
            GarageId = garageId;
            CarId = carId;
            EntryTime = DateTime.UtcNow;

            HourlyRate = hourlyRate;
            DailyRate = dailyRate;
            MonthlyRate = monthlyRate;

            Status = ParkingSessionStatus.Active;
        }

        public void RegisterPayment(decimal amount)
        {
            if (Status != ParkingSessionStatus.Active)
                throw new InvalidOperationException("Cannot pay closed session.");

            AmountPaid += amount;

            if (AmountPaid >= CalculateDueAmount())
                IsCleared = true;
        }

        public void Close()
        {
            if (Status != ParkingSessionStatus.Active)
                throw new InvalidOperationException("Session not active.");

            if (!IsCleared)
                throw new InvalidOperationException("Payment not cleared.");

            ExitTime = DateTime.UtcNow;
            Status = ParkingSessionStatus.Closed;
        }

        public decimal CalculateDueAmount()
        {
            var endTime = ExitTime ?? DateTime.UtcNow;
            var duration = endTime - EntryTime;

            var hours = (decimal)duration.TotalHours;

            return hours * HourlyRate;
        }
    }

}

