using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Data.Entities
{
    public class ParkingSession
    {
        [Key]
        public int Id { get; set; }

        public int GarageId { get; set; }
        public Garage Garage { get; set; } = null!;

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        /* Accounting: 
         * GarageFee = “What this garage charges today”
         * ParkingSession = “What THIS customer agreed to pay on entry”
         These are not the same object.
         */

        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal MonthlyRate { get; set; }

        public decimal AmountPaid { get; set; }
        public bool IsCleared { get; set; }
    }
}
