namespace CarsInsideGarage.Models.DTOs
{
    public class GarageCreateDto
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Area { get; set; } = null!;
        public string ParkingCoordinates { get; set; } = null!;
        public int ParkingFeeId { get; set; }
    }
}
