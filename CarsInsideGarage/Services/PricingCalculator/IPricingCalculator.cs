using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Services.PricingCalculator
{
    public interface IPricingCalculator
    {
        decimal CalculateTotal(ParkingSession session);
        
    }

}
