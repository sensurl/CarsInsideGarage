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

namespace CarsInsideGarage.Services.Garage
{
    public class GarageService : IGarageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

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
            var location = new CarsInsideGarage.Data.Entities.Location(
                Enum.Parse<Area>(dto.Area),
                dto.Lat, 
                dto.Lng);

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
                dto.Name, 
                dto.Capacity, 
                location, 
                policy, 
                user.UserId
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

            // Admin can delete any Garage
            // Owner can only delete their own Garages
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

        public async Task<CarsInsideGarage.Data.Entities.Garage?> GetNearestAsync(double lat, double lng)
        {
            return await _unitOfWork.Garages.GetNearestAsync(lat, lng);
        }

        public async Task<IEnumerable<CarsInsideGarage.Data.Entities.Garage>> GetNearestManyAsync(double lat, double lng, int count)
        {
            return await _unitOfWork.Garages.GetNearestManyAsync(lat, lng, count);
        }

    }
}
