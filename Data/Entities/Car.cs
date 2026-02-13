using CarsInsideGarage.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;

namespace CarsInsideGarage.Data.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        public string CarPlateNumber { get; set; } = null!;

        // LINK TO IDENTITY USER
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public virtual ICollection<ParkingSession> Sessions { get; set; } = new List<ParkingSession>();

    }
}
