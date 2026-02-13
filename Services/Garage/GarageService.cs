using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace CarsInsideGarage.Services.Garage
{
    public class GarageService : IGarageService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;
        public GarageService(GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GarageListDto>> GetAllAsync()
        {
            return await _context.Garages
                .ProjectTo<GarageListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task CreateAsync(GarageDetailsDto dto)
        {
            var parsedCoordinates = ParseCoordinates(dto.AddressCoordinates);

            var existing = await _context.AddressesCoordinates
     .FirstOrDefaultAsync(c =>
         c.Latitude == parsedCoordinates.Latitude &&
         c.Longitude == parsedCoordinates.Longitude);

            if (existing != null)
            {
                parsedCoordinates = existing;
            }
            else
            {
                _context.AddressesCoordinates.Add(parsedCoordinates);
                await _context.SaveChangesAsync();
            }


            var newLocation = new CarsInsideGarage.Data.Entities.Location
            {
                Area = Enum.Parse<Area>(dto.Area),
                AddressCoordinatesId = parsedCoordinates.Id
            };

            _context.Locations.Add(newLocation);
            await _context.SaveChangesAsync();

            var newGarage = _mapper.Map<CarsInsideGarage.Data.Entities.Garage>(dto);
            newGarage.LocationId = newLocation.Id;

            _context.Garages.Add(newGarage);
            await _context.SaveChangesAsync();
        }


        public async Task<GarageDetailsDto?> GetGarageDetailsAsync(int id)
        {
            var garage = await _context.Garages
                .Include(g => g.Sessions)
                .Include(g => g.Location)
                .ThenInclude(g => g.Coordinates)
                .Include(g => g.GarageFee)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null)
                return null;

            var dto = _mapper.Map<GarageDetailsDto>(garage);

            dto.TotalRevenue = CalculateTotalRevenue(garage.Sessions);

            return dto;
        }

        public async Task<GarageDetailsViewModel?> GetDetailsViewModelAsync(int garageId,bool isOwner)
        {
            var garage = await _context.Garages
                .Include(g => g.Location)
                .ThenInclude(l => l.Coordinates)
                .Include(g => g.GarageFee)
                .Include(g => g.Sessions)
                .FirstOrDefaultAsync(g => g.Id == garageId);

            if (garage == null) return null;

            var vm = new GarageDetailsViewModel
            {
                Id = garage.Id,
                Name = garage.Name,
                Area = garage.Location.Area,
                Coordinates = garage.Location.Coordinates.ToString(),
                FreeSpots = garage.Capacity
                    - garage.Sessions.Count(s => s.ExitTime == null),

                // Prices (always visible)
                HourlyRate = garage.GarageFee.HourlyRate,
                DailyRate = garage.GarageFee.DailyRate,
                MonthlyRate = garage.GarageFee.MonthlyRate,

                // Revenue (owner only)
                CanSeeRevenue = isOwner,
                TotalRevenue = isOwner
                    ? garage.Sessions.Sum(s => s.AmountPaid)
                    : 0
            };

            return vm;
        }


        public async Task<GarageDeleteConfirmationViewModel> DeleteGarageAsync(int id)
        {
            // Fetch with Location included
            var garage = await _context.Garages
                .Include(g => g.Location)
                .ThenInclude(l => l.Coordinates)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null) throw new Exception("Garage not found");

            // Capture the data into the VM before deleting
            var result = new GarageDeleteConfirmationViewModel
            {
                Name = garage.Name,
                Coordinates = garage.Location?.Coordinates.ToString() ?? "N/A"
            };

            // Remove the Garage (Cascade will handle the Location automatically)
            _context.Garages.Remove(garage);
            await _context.SaveChangesAsync();

            // Return the captured data
            return result;
        }


        /* Revenue = SUM of accrued revenue for all parking sessions (past + active)
         *Revenue is calculated from EntryTime → ExitTime (or NOW if still inside), 
         *Uses the snapshotted rates on the session
          */
        private decimal CalculateTotalRevenue(IEnumerable<ParkingSession> sessions)
        {
            var now = DateTime.UtcNow;
            decimal total = 0;

            foreach (var session in sessions)
            {
                var endTime = session.ExitTime ?? now;
                var duration = endTime - session.EntryTime;


                var totalHours = (decimal)duration.TotalHours;

                if (totalHours < 0)
                    continue;

                total += totalHours * session.HourlyRate;
            }

            return decimal.Round(total, 2);
        }

        private AddressCoordinates ParseCoordinates(string coordinates)
        {
            var parts = coordinates.Split(',', StringSplitOptions.TrimEntries);

            return new AddressCoordinates
            {
                Latitude = decimal.Parse(parts[0], CultureInfo.InvariantCulture),
                Longitude = decimal.Parse(parts[1], CultureInfo.InvariantCulture)

            };
        }

    }
}