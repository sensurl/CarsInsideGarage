using AutoMapper;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using CarsInsideGarage.Models.DTOs;
using CarsInsideGarage.Models.ViewModels;

namespace CarsInsideGarage.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =========================
            // Entity => DTO
            // =========================

            CreateMap<Garage, GarageListDto>()
                .ForMember(d => d.FreeSpots, opt =>
                    opt.MapFrom(g =>
                        g.Capacity - g.Sessions.Count(s => s.ExitTime == null)));

            CreateMap<Garage, GarageDetailsDto>()
                .ForMember(d => d.Area,
                    opt => opt.MapFrom(g =>
                        g.Location.Area.ToString()))
                .ForMember(d => d.ParkingCoordinates,
                    opt => opt.Ignore()) // handled in service
                .ForMember(d => d.HourlyRate,
                    opt => opt.MapFrom(g =>
                        g.PricingPolicy.HourlyRate))
                .ForMember(d => d.DailyRate,
                    opt => opt.MapFrom(g =>
                        g.PricingPolicy.DailyRate))
                .ForMember(d => d.MonthlyRate,
                    opt => opt.MapFrom(g =>
                        g.PricingPolicy.MonthlyRate))
                .ForMember(d => d.Rules,
                    opt => opt.MapFrom(g =>
                        g.PricingPolicy.Rules))
                .ForMember(d => d.ActiveCarsCount,
                    opt => opt.MapFrom(g =>
                        g.Sessions.Count(s => s.ExitTime == null)))
                .ForMember(d => d.TotalRevenue,
                    opt => opt.Ignore())
                .ForMember(d => d.ActiveSessions,
                    opt => opt.MapFrom(g =>
                        g.Sessions.Where(s => s.ExitTime == null)));

            CreateMap<Garage, RevenueReportDto>()
                .ForMember(dest => dest.GarageId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GarageName,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TotalRevenue,
                    opt => opt.MapFrom(src =>
                        src.Sessions.Where(s => s.ExitTime != null)
                                    .Sum(s => s.AmountPaid)));

            CreateMap<ParkingSession, ActiveSessionListDto>()
                .ForMember(dest => dest.SessionId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GarageName,
                    opt => opt.MapFrom(src => src.Garage.Name))
                .ForMember(dest => dest.CarPlateNumber,
                    opt => opt.MapFrom(src => src.Car.CarPlateNumber))
                .ForMember(dest => dest.DurationHours,
                    opt => opt.MapFrom(src =>
                        (DateTime.UtcNow - src.EntryTime).TotalHours));

            CreateMap<ParkingSession, SessionDto>()
                .ForMember(dest => dest.CarPlateNumber,
                    opt => opt.MapFrom(src => src.Car.CarPlateNumber))
                .ForMember(dest => dest.GarageName,
                    opt => opt.MapFrom(src => src.Garage.Name));
                
            CreateMap<Car, CarSelectionDto>()
                .ForMember(dest => dest.Id, opt => 
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CarPlateNumber, opt => 
                    opt.MapFrom(src => src.CarPlateNumber));

            CreateMap<PricingRule, PricingRuleDetailsDto>();

            // =========================
            // DTO => Entity
            // =========================

            CreateMap<SessionDto, ParkingSession>()
                .ForMember(dest => dest.Id, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.Garage, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.Car, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.Status, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.EntryTime, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.ExitTime, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.AmountPaid, opt => 
                     opt.Ignore())
                .ForMember(dest => dest.IsCleared, opt => 
                     opt.Ignore());


            // =========================
            // Entity <=> DTO
            // =========================

            CreateMap<Car, CarDto>().ReverseMap();

            // =========================
            // Entity => ViewModel
            // =========================

            CreateMap<ParkingSession, ActiveSessionListViewModel>()
                .ForMember(dest => dest.SessionId, opt => 
                     opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GarageName, opt => 
                     opt.MapFrom(src => src.Garage != null ? src.Garage.Name : "N/A"))
                .ForMember(dest => dest.CarPlateNumber, opt => 
                     opt.MapFrom(src => src.Car != null ? src.Car.CarPlateNumber : "N/A"))
                .ForMember(dest => dest.DurationHours, opt => 
                     opt.MapFrom(src => (DateTime.UtcNow - src.EntryTime).TotalHours))
                .ForMember(dest => dest.AmountPaid, opt => 
                     opt.MapFrom(src => src.AmountPaid));
             
            // =========================
            // DTO <=> ViewModel
            // =========================

            CreateMap<CarDto, CarViewModel>().ReverseMap();

            CreateMap<PricingRuleDetailsDto, PricingRuleCreateViewModel>().ReverseMap();

            // =========================
            // DTO => ViewModel
            // =========================

            CreateMap<GarageDetailsDto, GarageDetailsViewModel>()
                .ForMember(dest => dest.ParkingCoordinates, opt =>
                    opt.MapFrom(src => src.ParkingCoordinates))
                .ForMember(dest => dest.FreeSpots, opt =>
                    opt.MapFrom(src => src.Capacity - src.ActiveCarsCount))
                .ForMember(dest => dest.Area, opt =>
                    opt.MapFrom(src => Enum.Parse<Area>(src.Area)))
                .ForMember(dest => dest.CanSeeRevenue, opt =>
                    opt.MapFrom(src => true))
                .ForMember(dest => dest.TotalRevenue, opt =>
                    opt.MapFrom(src => src.TotalRevenue));

            CreateMap<SessionDto, SessionActiveViewModel>()
                .ForMember(dest => dest.AccruedAmount, opt =>
                    opt.Ignore());

            CreateMap<SessionDto, SessionDetailsViewModel>()
                .ForMember(dest => dest.TotalDue, opt => 
                    opt.Ignore());

            CreateMap<SessionDto, SessionHistoryViewModel>();

            CreateMap<ActiveSessionListDto, ActiveSessionListViewModel>();

            CreateMap<RevenueReportDto, RevenueReportViewModel>();

            CreateMap<GarageNearbyDto, GarageNearbyViewModel>();

            // =========================
            // ViewModel => DTO
            // =========================

            CreateMap<GarageCreateViewModel, GarageCreateDto>()
                .ForMember(dest => dest.Area, opt =>
                     opt.MapFrom(src => src.SelectedArea.ToString()));

            CreateMap<SessionCreateViewModel, SessionDto>()
                .ForMember(dest => dest.Id, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.GarageName, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.CarPlateNumber, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.EntryTime, opt =>
                    opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ExitTime, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.AmountPaid, opt =>
                    opt.MapFrom(src => 0m))
                .ForMember(dest => dest.IsCleared, opt =>
                    opt.MapFrom(src => false));

            CreateMap<PricingRuleCreateViewModel, PricingRuleCreateDto>();
        }
    }
}
