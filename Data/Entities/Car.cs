using CarsInsideGarage.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;

namespace CarsInsideGarage.Data.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        public string LicensePlate { get; set; } = null!;
        // Consider uppercasing and removing spaces before save
        // That belongs to a Service, not Entity.
    }
}
