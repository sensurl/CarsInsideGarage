using CarsInsideGarage.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class GarageDetailsViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public Area Area { get; set; }

        [Required]
        [StringLength(40)]
        public string ParkingCoordinates { get; set; } = null!;


        [Required]
        public int FreeSpots { get; set; }

        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }

        public bool CanSeeRevenue { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
