using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class SessionCreateViewModel
    {
        [Required]
        public int GarageId { get; set; }

        [Required]
        public int CarId { get; set; }

        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }
    }

}
