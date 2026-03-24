using CarsInsideGarage.Models.DTOs;

namespace CarsInsideGarage.Models.ViewModels
{
    public class SelectCarViewModel
    {
        public int GarageId { get; set; }
        public string GarageName { get; set; } = null!;
        public IEnumerable<CarSelectionDto> AvailableCars { get; set; } = new List<CarSelectionDto>();

    }
}
