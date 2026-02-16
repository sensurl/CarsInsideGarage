namespace CarsInsideGarage.Models.ViewModels
{
    public class SessionActiveViewModel
    {
        public int Id { get; set; }

        public string GarageName { get; set; } = null!;
        public string CarPlateNumber { get; set; } = null!;

        public DateTime EntryTime { get; set; }

        public decimal AccruedAmount { get; set; }
        public decimal AmountPaid { get; set; }

        public decimal RemainingAmount => AccruedAmount - AmountPaid;

        public bool CanExit => RemainingAmount <= 0;
    }
}
