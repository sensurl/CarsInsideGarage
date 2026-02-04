namespace CarsInsideGarage.Models.DTOs
{
    public class GarageDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }

        public string Area { get; set; } = null!;
        public string AddressCoordinates { get; set; } = null!;

        public decimal HourlyRate { get; set; }

        public int ActiveCarsCount { get; set; }

        public IEnumerable<SessionDto> ActiveSessions { get; set; }
            = new List<SessionDto>();
    }
}
