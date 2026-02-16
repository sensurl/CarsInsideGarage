using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CarsInsideGarage.Services.GarageSession
{
    public class ParkingSessionService : IParkingSessionService
    {
        private readonly GarageDbContext _context;
        private readonly IMapper _mapper;

        public ParkingSessionService(GarageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task StartSessionAsync(int garageId, int carId)
        {
            var garage = await _context.Garages
                .Include(g => g.Location)
                .Include(g => g.GarageFee)
                .FirstOrDefaultAsync(g => g.Id == garageId);

            if (garage == null)
                throw new Exception("Garage not found");

            var existingActive = await _context.ParkingSessions
                .AnyAsync(s => s.CarId == carId && s.ExitTime == null);

            if (existingActive)
                throw new Exception("Car already has an active session.");


            var session = new ParkingSession
            {
                GarageId = garageId,
                CarId = carId,
                EntryTime = DateTime.UtcNow,

                // Snapshot rates
                HourlyRate = garage.GarageFee.HourlyRate,
                DailyRate = garage.GarageFee.DailyRate,
                MonthlyRate = garage.GarageFee.MonthlyRate,

                AmountPaid = 0,
                IsCleared = false
            };

            _context.ParkingSessions.Add(session);
            await _context.SaveChangesAsync();
        }


        // ==========================================
        // READ
        // ==========================================

        public async Task<IEnumerable<SessionActiveViewModel>> GetActiveSessionsAsync(int garageId)
        {
            var sessions = await _context.ParkingSessions
                .Include(s => s.Car)
                .Include(s => s.Garage)
                .Where(s => s.GarageId == garageId && s.ExitTime == null)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<SessionDto>>(sessions);
            var viewModels = _mapper.Map<IEnumerable<SessionActiveViewModel>>(dtos);

            // Calculates the dynamic due amount using time-based logic
            foreach (var vm in viewModels)
            {
                var session = sessions.First(s => s.Id == vm.Id);
                vm.AccruedAmount = CalculateTotalDue(session);
            }

            return viewModels;
        }

        public async Task<int> GetCarIdBySessionId(int sessionId)
        {
            var carId = await _context.ParkingSessions
                .Where(s => s.Id == sessionId)
                .Select(s => s.CarId)
                .FirstOrDefaultAsync();

            if (carId == 0) // Default for int if not found
            {
                throw new Exception($"Session with ID {sessionId} not found.");
            }

            return carId;
        }

        public async Task<SessionActiveViewModel?> GetActiveSessionByCarAsync(int carId)
        {
            var sessionEntity = await _context.ParkingSessions
                .Include(s => s.Car)
                .Include(s => s.Garage)
                .FirstOrDefaultAsync(s => s.CarId == carId && s.ExitTime == null);

            if (sessionEntity == null)
                return null;

            var dto = _mapper.Map<SessionDto>(sessionEntity);
            var viewModel = _mapper.Map<SessionActiveViewModel>(dto);

            // Manual calculation of Business Logic 
            viewModel.AccruedAmount = CalculateTotalDue(sessionEntity);

            return viewModel;
        }


        // ==========================================
        // UPDATE
        // ==========================================

        private decimal CalculateTotalDue(ParkingSession session)
        {
            var now = DateTime.UtcNow;
            var endTime = session.ExitTime ?? now;
            var duration = endTime - session.EntryTime;

            var hours = (decimal)duration.TotalHours;

            if (hours < 0)
                return 0;

            return decimal.Round(hours * session.HourlyRate, 2);
        }

        public async Task PayAsync(int sessionId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Payment amount must be positive.");

            var session = await _context.ParkingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            session.AmountPaid += amount;

            await _context.SaveChangesAsync();
        }

        public async Task EndSessionAsync(int sessionId)
        {
            var session = await _context.ParkingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            var totalDue = CalculateTotalDue(session);

            if (session.AmountPaid < totalDue)
                throw new Exception("Outstanding balance must be paid before exit.");

            session.ExitTime = DateTime.UtcNow;
            session.IsCleared = true;

            await _context.SaveChangesAsync();
        }
    }
}