using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Services.PricingCalculator
{
    public class PricingCalculator : IPricingCalculator
    {
        public decimal CalculateTotal(ParkingSession session)
        {
            var endTime = session.ExitTime ?? DateTime.UtcNow;

            if (endTime <= session.EntryTime)
                return 0m;

            var hours = (decimal)(endTime - session.EntryTime).TotalHours;

            var total = hours * session.HourlyRate;

            return decimal.Round(total, 2);
        }
    }

}
