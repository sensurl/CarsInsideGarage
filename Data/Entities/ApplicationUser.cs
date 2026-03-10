using Microsoft.AspNetCore.Identity;

namespace CarsInsideGarage.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Car>? Cars { get; set; }
        public ICollection<Garage>? OwnedGarages { get; set; }
    }
}
