using CarsInsideGarage.Data.Enums;

namespace CarsInsideGarage.Models.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public Area Area { get; set; } 

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double DistanceInKm { get; set; }   // NEW
    }

}
