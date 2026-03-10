namespace CarsInsideGarage.Models.ViewModels
{
    public class PricingRuleCreateViewModel
    {
        public TimeSpan? StartHour { get; set; }
        public TimeSpan? EndHour { get; set; }

        public decimal? Multiplier { get; set; }
        public decimal? Adjustment { get; set; }
    }

}
