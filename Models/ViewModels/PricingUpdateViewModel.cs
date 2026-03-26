using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class PricingUpdateViewModel
    {
        public int GarageId { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        [Required]
        public decimal DailyRate { get; set; }

        [Required]
        public decimal MonthlyRate { get; set; }

        public List<PricingRuleDetailsViewModel>? Rules { get; set; }
    }

}
