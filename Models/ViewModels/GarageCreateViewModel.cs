using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class GarageCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Range(1, 300)]
        public int Capacity { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int GarageFeeId { get; set; }

        // For dropdowns
        public IEnumerable<SelectListItem> Locations { get; set; } = [];
        public IEnumerable<SelectListItem> Fees { get; set; } = [];
    }
}
