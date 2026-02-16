using CarsInsideGarage.Data.Enums;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Drawing;

namespace CarsInsideGarage.Data.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public Area Area { get; set; }

        // Spatial point
        public NetTopologySuite.Geometries.Point ParkingCoordinates { get; set; } = null!;


    }
}
