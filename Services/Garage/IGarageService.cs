using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Models.DTOs;

namespace CarsInsideGarage.Services.Garage
{
    public interface IGarageService
    {
        Task<IEnumerable<GarageListDto>> GetAllAsync();
        Task<GarageDetailsDto?> GetGarageDetailsAsync(int id);

        Task CreateAsync(
            string name,
            int capacity,
            Area area,
            string coordinates,
            int garageFeeId);
    }
}

