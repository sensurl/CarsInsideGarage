namespace CarsInsideGarage.Models.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }
        public int GarageId { get; set; }
        public string? GarageName { get; set; } // Added for the View

        public int CarId { get; set; }
        public string? CarPlateNumber { get; set; } // Added for the View

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsCleared { get; set; }

        // Rates (snapshots)
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }
    }
}
