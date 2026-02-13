namespace CarsInsideGarage.Models.DTOs
{
    public class RevenueReportDto
    {
        public int GarageId { get; set; }

        public string GarageName { get; set; } = null!;

        public decimal TotalRevenue { get; set; }
    }
}
