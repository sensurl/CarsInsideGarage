namespace CarsInsideGarage.Areas.Admin.Models
{
    public class DashboardStatsDto
    {
        public int TotalGarages { get; set; }
        public int TotalFreeSpotsAvailable { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalActiveSessions { get; set; }
    }
}
