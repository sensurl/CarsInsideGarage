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

        [Required(ErrorMessage = "You need to select a fee plan")]
        public int ParkingFeeId { get; set; }

        public IEnumerable<SelectListItem> Fees { get; set; } = [];
    }
}