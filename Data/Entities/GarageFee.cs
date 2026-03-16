using System.ComponentModel.DataAnnotations;

namespace CarsInsideGarage.Data.Entities
{
    public class GarageFee
    {
        [Key]
        public int Id { get; set; }

        public decimal HourlyRate { get; set; }

        public decimal DailyRate { get; set; }

        public decimal MonthlyRate { get; set; }

        public ICollection<Garage> Garages { get; set; } = new List<Garage>();
    }
}
