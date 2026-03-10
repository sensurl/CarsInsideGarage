using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class FeeCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Range(10, 50)]
        [Display(Name = "Daily Rate")]
        public decimal DailyRate { get; set; }

        [Required]
        [Range(30, 3000)]
        [Display(Name = "Monthly Rate")]
        public decimal MonthlyRate { get; set; }
    }
}