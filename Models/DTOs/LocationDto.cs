using CarsInsideGarage.Data.Enums;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CarsInsideGarage.Models.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public Area Area { get; set; }
        public string ParkingCoordinates { get; set; } = null!;
    }
}
