using CarsInsideGarage.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class GarageCreateViewModel
    {
        [Required(ErrorMessage = "Garage name is required")]
        public string Name { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }

        // Capture Area from the Enum
        [Required(ErrorMessage = "Please select an area")]
        public Area SelectedArea { get; set; }

        // Capture Coordinates string
        [Required(ErrorMessage = "Coordinates are required")]
        [Display(Name = "e.g 42.659892717892355, 23.315800826629413")]
        public string AddressCoordinates { get; set; } = null!;

        [Required(ErrorMessage = "You need to select a fee plan")]
        public int SelectedFeeId { get; set; }

        public IEnumerable<SelectListItem> Fees { get; set; } = [];
    }
}