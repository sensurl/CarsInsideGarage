namespace CarsInsideGarage.Models.ViewModels
{
    public class SessionDetailsViewModel
    {
        public int Id { get; set; }

        public string GarageName { get; set; } = null!;

        public string CarPlateNumber { get; set; } = null!;

        public DateTime EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        public decimal TotalDue { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal Remaining => TotalDue - AmountPaid;
    }
}
