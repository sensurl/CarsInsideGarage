using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Data.Entities
{
    public class Garage
    {
        private Garage() { }   // EF Core needs this

        public Garage(string name, int capacity, int locationId, PricingPolicy pricingPolicy, string userId)
        {
            Name = name;
            Capacity = capacity;
            LocationId = locationId;
            PricingPolicy = pricingPolicy;
            UserId = userId;
        }


        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public PricingPolicy PricingPolicy { get; private set; } = null!;

        // One Garage has many active/past parking sessions
        public ICollection<ParkingSession> Sessions { get; set; } = new HashSet<ParkingSession>();

        // Cars inside a garage are derived from active sessions, not a direct relationship.
        // Garage <= ParkingSession => Car

        // LINK TO IDENTITY USER
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // Soft delete properties
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedByUserId { get; set; }


    }
}
