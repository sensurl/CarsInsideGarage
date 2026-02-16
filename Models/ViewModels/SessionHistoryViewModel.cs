namespace CarsInsideGarage.Models.ViewModels
{
    public class SessionHistoryViewModel
    {
        public int Id { get; set; }
        public string GarageName { get; set; } = null!;
        public string CarPlateNumber { get; set; } = null!;
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
