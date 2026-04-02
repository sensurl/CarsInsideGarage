using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Services.Time;
using System.Security.Cryptography;

namespace CarsInsideGarage.Services.PricingCalculator
{
    public class PricingCalculator : IPricingCalculator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        public PricingCalculator(IDateTimeProvider dateTimeProvider) 
        { 
            _dateTimeProvider = dateTimeProvider;
        }
        public decimal CalculateTotal(ParkingSession session)
        {
            var endTime = session.ExitTime ?? _dateTimeProvider.UtcNow;

            if (endTime <= session.EntryTime)
                return 0m;

            var hours = (decimal)(endTime - session.EntryTime).TotalHours;

            var total = hours * session.HourlyRate;

            return decimal.Round(total, 2);
        }
    }

}
