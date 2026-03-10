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

        public async Task StartSessionAsync(int garageId, string userId)
        {
            var garage = await _unitOfWork.Garages.GetByIdAsync(garageId);

            if (garage == null)
                throw new Exception("Garage not found");

            var car = await _unitOfWork.Cars
            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (car == null)
                throw new Exception("Driver has no registered car.");

            var existingActive = await _unitOfWork.Sessions
            .GetActiveSessionByCarIdAsync(car.Id);

            if (existingActive != null)
                throw new Exception("Car already has active session.");

            var policy = garage.PricingPolicy;

            if (policy == null)
                throw new Exception("Garage has no pricing policy.");

            // 🔥 SNAPSHOT RATE CALCULATED ONCE
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

       
        public async Task<SessionActiveViewModel?> GetActiveSessionForDriverAsync(string userId)
        {
            var car = await _unitOfWork.Cars
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (car == null)
                return null;

            var session = await _unitOfWork.Sessions
                .GetActiveSessionByCarIdAsync(car.Id);

            if (session == null)
                return null;

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

        public async Task<IEnumerable<SessionActiveViewModel>> GetActiveSessionsForGarageOwnerAsync(string userId)
        {
            var garages = await _unitOfWork.Garages
                .WhereAsync(g => g.UserId == userId);

            var garageIds = garages.Select(g => g.Id).ToList();

            var allSessions = new List<ParkingSession>();

            foreach (var garageId in garageIds)
            {
                var sessions = await _unitOfWork.Sessions
                    .GetActiveSessionsByGarageIdAsync(garageId);

                allSessions.AddRange(sessions);
            }

            var dtos = _mapper.Map<List<SessionDto>>(allSessions);
            var viewModels = _mapper.Map<List<SessionActiveViewModel>>(dtos);

            foreach (var vm in viewModels)
            {
                var entity = allSessions.First(s => s.Id == vm.Id);
                vm.AccruedAmount = _pricingCalculator.CalculateTotal(entity);

            }

            return viewModels;
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

       
        
    }
}