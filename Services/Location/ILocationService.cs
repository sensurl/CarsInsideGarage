using CarsInsideGarage.Models.DTOs;

namespace CarsInsideGarage.Services.Location
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllAsync();
        Task<LocationDto?> GetByIdAsync(int id);
    }
}