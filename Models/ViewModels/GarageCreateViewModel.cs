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

        [Required(ErrorMessage = "Latitude is required")]
        [Display(Name = "Latitude (e.g. 42.6977)")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Lat { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Display(Name = "Longitude (e.g. 23.3219)")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Lng { get; set; }



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