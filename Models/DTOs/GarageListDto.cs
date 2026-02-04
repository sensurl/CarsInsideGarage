using CarsInsideGarage.Models;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using System.Collections.Generic;


namespace CarsInsideGarage.Models.DTOs
{
    public class GarageListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public int FreeSpots { get; set; }

    }
}











