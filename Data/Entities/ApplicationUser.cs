using CarsInsideGarage.Interfaces;
using Microsoft.AspNetCore.Identity;
using CarsInsideGarage.Interfaces;

namespace CarsInsideGarage.Data.Entities
{
    public class ApplicationUser : IdentityUser, ISoftDeletable
    {
        public ICollection<Car>? Cars { get; set; }
        public ICollection<Garage>? OwnedGarages { get; set; }
        public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
