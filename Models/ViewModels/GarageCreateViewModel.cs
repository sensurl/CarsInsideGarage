using CarsInsideGarage.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CarsInsideGarage.Models.ViewModels
{
    public class GarageCreateViewModel
    {
        [Required(ErrorMessage = "Garage name is required")]
        public string Name { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }


        [Required(ErrorMessage = "Please select an area")]
        public Area SelectedArea { get; set; }


        [Required(ErrorMessage = "Coordinates are required")]
        [Display(Name = "e.g 42.659892717892355, 23.315800826629413")]
        [RegularExpression(@"^-?\d+(\.\d+)?,\s*-?\d+(\.\d+)?$", ErrorMessage = "Coordinates must be in 'lat,lng' format.")]
        public string ParkingCoordinates { get; set; } = null!;

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


        public List<PricingRuleCreateViewModel> Rules { get; set; }
       = new();
    }
}