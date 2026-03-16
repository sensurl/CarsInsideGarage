namespace CarsInsideGarage.Models.DTOs
{
    public class GarageCreateDto
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Area { get; set; } = null!;
        public string ParkingCoordinates { get; set; } = null!;
        // PricingPolicy snapshot
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }

        public List<PricingRuleCreateDto> Rules { get; set; } = new();
    }
}
