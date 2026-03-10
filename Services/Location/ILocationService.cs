using CarsInsideGarage.Models.DTOs;
using System.Linq.Dynamic.Core;

namespace CarsInsideGarage.Services.Location
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllAsync();
        Task<LocationDto?> GetByIdAsync(int id);

        Task<PagedResult<LocationDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusInKm,
        int pageNumber,
        int pageSize);
    }
}