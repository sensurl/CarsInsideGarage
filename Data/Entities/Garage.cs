using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Data.Entities
{
    public class Garage
    {
        [Key]
        public int Id { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int Capacity { get; set; } 

        public int GarageFeeId { get; set; }
        public GarageFee GarageFee { get; set; } = null!;


        // Linkage: One Garage has many active/past parking sessions
        public ICollection<ParkingSession> Sessions { get; set; } = new HashSet<ParkingSession>();

        // Cars inside a garage are derived from active sessions, not a direct relationship.
        // Garage ← ParkingSession → Car

    }
}
