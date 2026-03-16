
namespace CarsInsideGarage.Models.DTOs
{
    public class GarageDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }

        public string Area { get; set; } = null!;

        // Spatial Point coordinates formatted as "lat, lng"
        public string ParkingCoordinates { get; set; } = null!;

        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }


        public IEnumerable<PricingRuleDetailsDto> Rules { get; set; }
        = new List<PricingRuleDetailsDto>();

        public int ActiveCarsCount { get; set; }

        public decimal TotalRevenue { get; set; }

        public IEnumerable<SessionDto> ActiveSessions { get; set; } = new List<SessionDto>();
    }
}
