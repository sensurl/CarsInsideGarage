using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Models.Auth;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;
using CarsInsideGarage.Services.PricingCalculator;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace CarsInsideGarage.Services.GarageSession
{
    public class ParkingSessionService : IParkingSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPricingCalculator _pricingCalculator;

        public ParkingSessionService(IUnitOfWork unitOfWork, IMapper mapper, IPricingCalculator pricingCalculator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pricingCalculator = pricingCalculator;
        }

        // ==========================================
        // START SESSION (Driver Only - Secure)
        // ==========================================

        public async Task StartSessionAsync(int garageId, int carId, string userId)
        {
            var garage = await _unitOfWork.Garages.GetByIdAsync(garageId);

            if (garage == null)
                throw new Exception("Garage not found");

            //var car = await _unitOfWork.Cars
            //.FirstOrDefaultAsync(c => c.UserId == userId);

            var car = await _unitOfWork.Cars.GetByIdAsync(carId);

            if (car == null)
                throw new Exception("Driver has no registered car.");

            if (car.UserId != userId)
                throw new UnauthorizedAccessException("This Car does not belong to this Driver.");

            var existingActive = await _unitOfWork.Sessions
            .GetActiveSessionByCarIdAsync(car.Id);

            if (existingActive != null)
                throw new Exception($"Car already parked in Garage: {garage.Name}.");

            var policy = garage.PricingPolicy;

            if (policy == null)
                throw new Exception("Garage has no pricing policy.");

            // SNAPSHOT RATE CALCULATED ONCE
            var effectiveHourlyRate =
                policy.GetEffectiveHourlyRate(DateTime.UtcNow);

            var session = new ParkingSession(
                garage.Id,
                car.Id,
                effectiveHourlyRate,
                policy.DailyRate,
                policy.MonthlyRate);

            await _unitOfWork.Sessions.AddAsync(session);
            await _unitOfWork.CompleteAsync();
        }


        // ==========================================
        // DRIVER ACTIVE SESSION (Secure)
        // ==========================================


        public async Task<SessionActiveViewModel?> GetActiveSessionDetailsAsync(int sessionId, string userId)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);

            if (session == null || session.ExitTime != null) return null;

            var car = await _unitOfWork.Cars.GetByIdAsync(session.CarId);

            if (car == null || car.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to view this session.");

            // Using the helper
            return MapToActiveVm(session);
        }

        public async Task<IEnumerable<SessionActiveViewModel?>> GetActiveSessionsForDriverAsync(string userId)
        {
            // 1. Get all cars belonging to the user
            var userCars = await _unitOfWork.Cars.WhereAsync(c => c.UserId == userId && !c.IsDeleted);
            var carIds = userCars.Select(c => c.Id).ToList();

            // 2. Get all active sessions for those car IDs
            var activeSessions = await _unitOfWork.Sessions
                .WhereAsync(s => carIds.Contains(s.CarId) && s.ExitTime == null);

            // 3. Map to DTO => ViewModels
            var vms = new List<SessionActiveViewModel>();

            //foreach (var session in activeSessions)
            //{
            //    var dto = _mapper.Map<SessionDto>(session);
            //    var vm = _mapper.Map<SessionActiveViewModel>(dto);
            //    vm.AccruedAmount = _pricingCalculator.CalculateTotal(session);
            //    vms.Add(vm);
            //}

            //return vms;

            // Using the helper
            return activeSessions.Select(MapToActiveVm);
        }

        // DRY helper
        private SessionActiveViewModel MapToActiveVm(ParkingSession session)
        {
            var dto = _mapper.Map<SessionDto>(session);
            var vm = _mapper.Map<SessionActiveViewModel>(dto);
            vm.AccruedAmount = _pricingCalculator.CalculateTotal(session);
            return vm;
        }

        public async Task<int> GetCarIdBySessionIdAsync(int sessionId)
        {
            var session = await _unitOfWork.Sessions
            .GetByIdAsync(sessionId);

            if (session == null)
                throw new Exception("Session not found");

            if (session.CarId == 0) // Default for int if not found
            {
                throw new Exception($"Car with ID {session.CarId} not found.");
            }

            return session.CarId;
        }

        // =====================================================
        // OWNER ACTIVE LIST (Secure Replacement for ActiveList)
        // =====================================================

        public async Task<IEnumerable<ActiveSessionListViewModel>> GetActiveSessionsForGarageOwnerAsync(string userId)
            {
            var garages = await _unitOfWork.Garages.WhereAsync(g => g.UserId == userId);
            var garageIds = garages.Select(g => g.Id).ToList();

            var allSessions = new List<ParkingSession>();
            foreach (var garageId in garageIds)
                {
                var sessions = await _unitOfWork.Sessions.GetActiveSessionsByGarageIdAsync(garageId);
                allSessions.AddRange(sessions);
                }

            return _mapper.Map<IEnumerable<ActiveSessionListViewModel>>(allSessions);
            }

        // ==========================================
        // PAY (Driver Secure)
        // ==========================================

        public async Task PayAsync(int sessionId, decimal amount, string userId)
        {
            if (amount <= 0)
                throw new Exception("Payment amount must be positive.");

            var session = await _unitOfWork.Sessions
                .GetByIdAsync(sessionId);

            if (session == null)
                throw new Exception("Session not found");

            var car = await _unitOfWork.Cars.GetByIdAsync(session.CarId);
            if (car == null || car.UserId != userId)
                throw new UnauthorizedAccessException();

            session.RegisterPayment(amount);

            _unitOfWork.Sessions.Update(session);

            await _unitOfWork.CompleteAsync();
        }

        // =====================================================
        // EXIT (Driver Secure)
        // =====================================================

        public async Task EndSessionAsync(int sessionId, string userId)
        {
            var session = await _unitOfWork.Sessions
                .GetByIdAsync(sessionId);

            if (session == null)
                throw new Exception("Session not found");

            var car = await _unitOfWork.Cars
                .GetByIdAsync(session.CarId);

            if (car == null || car.UserId != userId)
                throw new UnauthorizedAccessException();

            var totalDue = _pricingCalculator.CalculateTotal(session);

            if (session.AmountPaid < totalDue)
                throw new Exception("Outstanding balance must be paid before exit.");

            session.Close();

            _unitOfWork.Sessions.Update(session);

            await _unitOfWork.CompleteAsync();
        }

        // =====================================================
        // OWNER STOP
        // =====================================================

        public async Task<bool> StopSessionAsync(int sessionId, CurrentUser user)
        {
            var session = await _unitOfWork.Sessions
                .GetByIdAsync(sessionId);

            if (session == null)
                return false;

            var garage = await _unitOfWork.Garages
                .GetByIdAsync(session.GarageId);

            if (garage == null)
                return false;

            bool isOwner = user.IsOwner && garage.UserId == user.UserId;
            bool isAdmin = user.IsAdmin;

            if (!isOwner && !isAdmin)
                return false;

            session.Close();


            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.CompleteAsync();

            return true;
        }



        // =====================================================
        // ADMIN DASHBOARD - ALL ACTIVE SESSIONS (Admin Only)
        // =====================================================

        public async Task<IEnumerable<ParkingSession>> GetAllActiveSessionsAsync()
        {
            return await _unitOfWork.Sessions.WhereAsync(s => s.ExitTime == null);
        }

    }
}