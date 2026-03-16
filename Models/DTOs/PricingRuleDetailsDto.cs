namespace CarsInsideGarage.Models.DTOs
{
    public class PricingRuleDetailsDto
    {
        public TimeSpan? StartHour { get; set; }
        public TimeSpan? EndHour { get; set; }

        public decimal? Multiplier { get; set; }
        public decimal? Adjustment { get; set; }
    }

}
