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

        public string CarPlateNumber { get; set; } = null!;

    }
}
