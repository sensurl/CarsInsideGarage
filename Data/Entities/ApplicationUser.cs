using CarsInsideGarage.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace CarsInsideGarage.Data.Entities
{
    public class ApplicationUser : IdentityUser, ISoftDeletable
    {
        public ICollection<Car>? Cars { get; set; }
        public ICollection<Garage>? OwnedGarages { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        }
}
