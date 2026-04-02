namespace CarsInsideGarage.Models.DTOs
{
    public class PricingRuleCreateDto
    {
        public TimeSpan? StartHour { get; set; }
        public TimeSpan? EndHour { get; set; }

        public decimal? Multiplier { get; set; }
        public decimal? Adjustment { get; set; }
    }

}
