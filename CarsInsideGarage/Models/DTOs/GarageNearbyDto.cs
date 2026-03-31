namespace CarsInsideGarage.Models.DTOs
{
    public class GarageNearbyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int FreeSpots { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Distance { get; set; } 
    }
}
