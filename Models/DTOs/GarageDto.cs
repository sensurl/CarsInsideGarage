using CarsInsideGarage.Models;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using System.Collections.Generic;


namespace CarsInsideGarage.Models.DTOs
{
    public class GarageDto
    {

        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        public int GarageFeeId { get; set; }
        public ICollection<SessionDto> Sessions { get; set; } = new HashSet<SessionDto>();

    }
}











