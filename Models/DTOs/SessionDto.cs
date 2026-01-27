using CarsInsideGarage.Data.Entities;

namespace CarsInsideGarage.Models.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }

        public int GarageId { get; set; }
       

        public int CarId { get; set; }
        
        public DateTime EntryTime { get; set; }

        
        public DateTime? ExitTime { get; set; }

        public decimal TotalAmountPaid { get; set; }
        public bool IsCleared { get; set; }
    }
}
