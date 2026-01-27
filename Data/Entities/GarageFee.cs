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
    }
}
