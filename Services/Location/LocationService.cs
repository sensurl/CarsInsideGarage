using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.DTOs;
using System.Linq.Dynamic.Core;

namespace CarsInsideGarage.Services.Location
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllAsync()
        {
            var locations = await _unitOfWork.Locations.GetAllAsync();
            return _mapper.Map<List<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Locations.GetByIdAsync(id);

            if (entity == null)
                return null;

            return _mapper.Map<LocationDto>(entity);
        }

        // ====================================================
        // NEAREST LOCATIONS WITH PAGING (HAVERSINE)
        // ====================================================

        public async Task<PagedResult<LocationDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            double radiusInKm,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize <= 0)
                pageSize = 10;

            var allLocations = await _unitOfWork.Locations.GetAllAsync();

            var filtered = allLocations
                .Select(location =>
                {
                    var locLat = location.ParkingCoordinates.Y;
                    var locLon = location.ParkingCoordinates.X;

                    var distance = CalculateDistanceKm(
                        latitude,
                        longitude,
                        locLat,
                        locLon);

                    return new
                    {
                        Location = location,
                        Distance = distance
                    };
                })
                .Where(x => x.Distance <= radiusInKm)
                .OrderBy(x => x.Distance)
                .ToList();

            var totalCount = filtered.Count;

            var pagedItems = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x =>
                {
                    var dto = _mapper.Map<LocationDto>(x.Location);
                    dto.DistanceInKm = Math.Round(x.Distance, 2);
                    return dto;
                })
                .ToList();

            return new PagedResult<LocationDto>
            {
                CurrentPage = pageNumber,
                PageCount = totalCount,
                Queryable = pagedItems.AsQueryable(),
                RowCount = pagedItems.Count,
                PageSize = pageSize
            };
        }

        // ====================================================
        // HAVERSINE
        // ====================================================

        private static double CalculateDistanceKm(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            const double earthRadiusKm = 6371.0;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) *
                Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
