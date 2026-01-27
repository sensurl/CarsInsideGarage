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

        //You need a way to distinguish between a car that is "currently inside" and a car that "visited last week." Using ExitTime == null is the standard way to do this
        public DateTime? ExitTime { get; set; } 

        public decimal TotalAmountPaid { get; set; }
        public bool IsCleared { get; set; } // For your requirement: "cannot leave unless clean"

       // Garage ← ParkingSession → Car

    }
}
