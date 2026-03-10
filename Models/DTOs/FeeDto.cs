namespace CarsInsideGarage.Models.DTOs
{
    public class FeeDto
    {
        public int Id { get; set; }

        public decimal HourlyRate { get; set; }

        public decimal DailyRate { get; set; }

        public decimal MonthlyRate { get; set; }
    }
}
