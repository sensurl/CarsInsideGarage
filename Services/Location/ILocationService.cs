using CarsInsideGarage.Models.DTOs;
using System.Collections.Generic;

namespace CarsInsideGarage.Services.Location
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllAsync();

        Task<LocationDto?> GetByIdAsync(int id);
    }
}