using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Runtime.ConstrainedExecution;

namespace CarsInsideGarage.Services.Garage
{
    public class GarageService : IGarageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        //private readonly GarageDbContext _context;
        

        public GarageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
           _unitOfWork = unitOfWork;
            _mapper = mapper;  
        }

        // ================================
        // LIST
        // ================================

        public async Task<IEnumerable<GarageListDto>> GetAllAsync(CurrentUser user)
        {
            //// 1. Get the current logged-in user
            //var user = _httpContextAccessor.HttpContext?.User;
            //var userId = _userManager.GetUserId(user);

            //// In case of user not logged in (no user), return an empty list 

            //if (user == null || !user.Identity.IsAuthenticated)
            //    {
            //    return Enumerable.Empty<GarageListDto>();
            //    }

            //// 2. Start the query
            //var query = _context.Garages.AsQueryable();

            //// 3. Filter: If they aren't an Admin, only show their own garages
            //if (!user.IsInRole("Admin"))
            //    {
            //    query = query.Where(g => g.UserId == userId);
            //    }

            //// 4. Project and return
            //return await query
            //    .ProjectTo<GarageListDto>(_mapper.ConfigurationProvider)
            //    .ToListAsync();

            var garages = await _unitOfWork.Garages.GetAllAsync();

               garages = user.IsAdmin
                    ? garages
                    : garages.Where(g => g.UserId == user.UserId);

            return _mapper.Map<List<GarageListDto>>(garages);
        }


        // ================================
        // CREATE
        // ================================

        public async Task<int> CreateAsync(GarageCreateDto dto, CurrentUser user)
        {

            var location = new CarsInsideGarage.Data.Entities.Location
            {
                Area = Enum.Parse<Area>(dto.Area),
                ParkingCoordinates = ParseCoordinates(dto.ParkingCoordinates)
            };

            await _unitOfWork.Locations.AddAsync(location);
            await _unitOfWork.CompleteAsync();

// Build pricing policy
    var policy = new PricingPolicy(
        dto.HourlyRate,
        dto.DailyRate,
        dto.MonthlyRate
    );
	
	if (dto.Rules != null)
    {
        foreach (var rule in dto.Rules)
        {
            policy.AddRule(new PricingRule(
                rule.StartHour,
                rule.EndHour,
                rule.Multiplier,
                rule.Adjustment
            ));
        }
    }
	
            var garage = new CarsInsideGarage.Data.Entities.Garage
            (
                dto.Name, dto.Capacity, location.Id, policy,user.UserId
            );

            await _unitOfWork.Garages.AddAsync(garage);
            await _unitOfWork.CompleteAsync();

            return garage.Id;
        }

        // ================================
        // DETAILS DTO
        // ================================

        public async Task<GarageDetailsDto?> GetGarageDetailsAsync(int id)
        {
            var garage = await _unitOfWork.Garages.GetGarageWithDetailsAsync(id);

            if (garage == null)
                return null;

            var dto = _mapper.Map<GarageDetailsDto>(garage);

            dto.TotalRevenue = CalculateTotalRevenue(garage.Sessions);

            // Convert Point → string
            dto.ParkingCoordinates = FormatPoint(garage.Location.ParkingCoordinates);

            return dto;
        }


        // ================================
        // DETAILS VIEW MODEL
        // ================================

        public async Task<GarageDetailsViewModel?> GetDetailsViewModelAsync(int id, CurrentUser user)
        {
            var garage = await _unitOfWork.Garages.GetGarageWithDetailsAsync(id);

            if (garage == null) 
                return null;

            return new GarageDetailsViewModel
            {
                Id = garage.Id,
                Name = garage.Name,
                Area = garage.Location.Area,
                ParkingCoordinates = FormatPoint(garage.Location.ParkingCoordinates),

                FreeSpots = garage.Capacity -
                            garage.Sessions.Count(s => s.ExitTime == null),

                HourlyRate = garage.PricingPolicy.HourlyRate,
DailyRate = garage.PricingPolicy.DailyRate,
MonthlyRate = garage.PricingPolicy.MonthlyRate,


                CanSeeRevenue = user.IsOwner,
                TotalRevenue = user.IsOwner
                    ? CalculateTotalRevenue(garage.Sessions)
                    : 0
            };
        }

        // ================================
        // DELETE
        // ================================

        public async Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id, CurrentUser user)
        {

            var garage = await _unitOfWork.Garages.GetGarageWithDetailsAsync(id);

            if (garage == null)
                throw new Exception("Garage not found");

            var result = new GarageDeleteConfirmationViewModel
            {
                Name = garage.Name,
                ParkingCoordinates = FormatPoint(garage.Location.ParkingCoordinates)
            };

            // Admin can delete any car
            // Driver can delete only their own car
            if (!user.IsAdmin && !user.IsOwner)
                throw new UnauthorizedAccessException();


            if (!user.IsAdmin && garage.UserId != user.UserId)
                throw new UnauthorizedAccessException();


            _unitOfWork.Garages.Remove(garage);
            await _unitOfWork.CompleteAsync();

            return result;
        }


        // ================================
        // HELPERS
        // ================================

        private string FormatPoint(NetTopologySuite.Geometries.Point point)
        {
            return $"{point.Y.ToString(CultureInfo.InvariantCulture)}, " +
                   $"{point.X.ToString(CultureInfo.InvariantCulture)}";
        }

        private NetTopologySuite.Geometries.Point ParseCoordinates(string coordinates)
        {
            var parts = coordinates.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                throw new ArgumentException("Invalid coordinates format. Expected 'lat,lng'.");

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                throw new ArgumentException("Latitude is invalid.");

            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lng))
                throw new ArgumentException("Longitude is invalid.");

            var point = _geometryFactory.CreatePoint(new Coordinate(lng, lat));
            point.SRID = 4326;
            return point;
        }


        private decimal CalculateTotalRevenue(IEnumerable<ParkingSession> sessions)
        {
            var now = DateTime.UtcNow;
            decimal total = 0;

            foreach (var session in sessions)
            {
                var endTime = session.ExitTime ?? now;
                var hours = (decimal)(endTime - session.EntryTime).TotalHours;

                if (hours > 0)
                    total += hours * session.HourlyRate;
            }

            return decimal.Round(total, 2);
        }
    }
}
