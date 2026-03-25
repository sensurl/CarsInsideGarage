using CarsInsideGarage.Data.Enums;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CarsInsideGarage.Data.Entities
{
    public class Location
    {

        public Area Area { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        // Spatial point only used for queries
        public NetTopologySuite.Geometries.Point ParkingCoordinates { get; private set; } = null!; 
        
        public Location() { } // EF

        public Location(Area area, double lat, double lng)
        {
            Area = area;
            Latitude = lat;
            Longitude = lng;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            ParkingCoordinates.SRID = 4326;
            ParkingCoordinates = geometryFactory.CreatePoint(new Coordinate(lng, lat));
        }

    }
}
