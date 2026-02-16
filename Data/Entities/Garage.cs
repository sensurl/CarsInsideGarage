using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Data.Entities
{
    public class Garage
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int ParkingFeeId { get; set; }
        public GarageFee GarageFee { get; set; } = null!;


        // One Garage has many active/past parking sessions
        public ICollection<ParkingSession> Sessions { get; set; } = new HashSet<ParkingSession>();

        // Cars inside a garage are derived from active sessions, not a direct relationship.
        // Garage <= ParkingSession => Car

        // LINK TO IDENTITY USER
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
