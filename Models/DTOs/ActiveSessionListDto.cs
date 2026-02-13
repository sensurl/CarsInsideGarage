namespace CarsInsideGarage.Models.DTOs
{
    public class ActiveSessionListDto
    {
        public int SessionId { get; set; }

        public string GarageName { get; set; } = null!;

        public string CarPlateNumber { get; set; } = null!;

        public DateTime EntryTime { get; set; }

        public decimal AmountPaid { get; set; }

        public double DurationHours { get; set; }
    }
}
