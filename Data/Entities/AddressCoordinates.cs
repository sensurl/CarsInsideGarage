namespace CarsInsideGarage.Data.Entities
{
    public class AddressCoordinates
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // for debugging and simple UI displays
        public override string ToString() => $"{Latitude:F4}, {Longitude:F4}";

    }
}
