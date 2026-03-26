namespace CarsInsideGarage.Models.DTOs
{
    public class PricingUpdateDto
    {
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }

        public List<PricingRuleDetailsDto>? Rules { get; set; }
    }
}
