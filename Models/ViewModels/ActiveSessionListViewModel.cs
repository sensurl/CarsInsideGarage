namespace CarsInsideGarage.Models.ViewModels
{
    public class ActiveSessionListViewModel
    {
        public int SessionId { get; set; }

        public string GarageName { get; set; } = null!;

        public string CarPlateNumber { get; set; } = null!;

        public DateTime EntryTime { get; set; }

        public decimal AmountPaid { get; set; }

        // Calculated in GarageSession/ParkingSessionService
        public double DurationHours { get; set; }
    }
}
