using CarsInsideGarage.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Models.ViewModels
{
    public class GarageNearbyViewModel
    {
     
            public int Id { get; set; }

            public string Name { get; set; } = null!;

            public int FreeSpots { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public double Distance { get; set; }

            
            public string DistanceFormatted => $"{Distance:F2} km";
            }
        
}
