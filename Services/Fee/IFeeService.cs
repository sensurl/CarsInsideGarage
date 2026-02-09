using CarsInsideGarage.Models.DTOs;

namespace CarsInsideGarage.Services.Fee
{
    public interface IFeeService
    {
        // Returns all fee structures for the dropdown
        Task<IEnumerable<FeeDto>> GetAllAsync();

        Task<FeeDto?> GetByIdAsync(int id);
    }
}