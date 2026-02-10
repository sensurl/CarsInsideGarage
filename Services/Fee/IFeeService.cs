using CarsInsideGarage.Models.DTOs;

namespace CarsInsideGarage.Services.Fee
{
    public interface IFeeService
    {
        // Returns all fee structures for the dropdown
        Task<IEnumerable<FeeDto>> GetAllAsync();
        Task CreateAsync(FeeDto feeDto);
        Task DeleteAsync(int id);

    }
}