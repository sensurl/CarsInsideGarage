namespace CarsInsideGarage.Models.ViewModels
{
    public class RevenueReportViewModel
    {
        public int GarageId { get; set; }

        public string GarageName { get; set; } = null!;

        public decimal TotalRevenue { get; set; }
    }
}
