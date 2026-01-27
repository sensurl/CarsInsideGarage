using CarsInsideGarage.Data.Enums;

namespace CarsInsideGarage.Data.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public Area Area { get; set; }
        public string AddressCoordinates { get; set; } = null!;

    }
}
