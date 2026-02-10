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
            // Entity → DTO
            CreateMap<Garage, GarageListDto>()
                .ForMember(
                    d => d.FreeSpots,
                    opt => opt.MapFrom(
                        g => g.Capacity - g.Sessions.Count(s => s.ExitTime == null)));

            CreateMap<Garage, GarageDetailsDto>()
        .ForMember(d => d.Area, opt => opt.MapFrom(g => g.Location.Area.ToString()))
        .ForMember(d => d.AddressCoordinates, opt => opt.MapFrom(g => g.Location.AddressCoordinates))
        .ForMember(d => d.HourlyRate, opt => opt.MapFrom(g => g.GarageFee.HourlyRate))
        .ForMember(d => d.DailyRate, opt => opt.MapFrom(g => g.GarageFee.DailyRate))
        .ForMember(d => d.MonthlyRate, opt => opt.MapFrom(g => g.GarageFee.MonthlyRate))
        .ForMember(d => d.ActiveCarsCount, opt => opt.MapFrom(g => g.Sessions.Count(s => s.ExitTime == null)))
        .ForMember(d => d.TotalRevenue, opt => opt.MapFrom(g => g.Sessions.Where(s => s.ExitTime != null).Sum(s => s.AmountPaid)))
        .ForMember(d => d.ActiveSessions, opt => opt.MapFrom(g => g.Sessions.Where(s => s.ExitTime == null)));

            //  DTO <=> Entity
            CreateMap<Car, CarDto>().ReverseMap();


            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<GarageFee, FeeDto>().ReverseMap();
            CreateMap<ParkingSession, SessionDto>().ReverseMap();

            // DTO <=> ViewModel
            CreateMap<CarDto, CarViewModel>().ReverseMap();
            CreateMap<FeeDto, FeeCreateViewModel>().ReverseMap();

            CreateMap<GarageDetailsDto, GarageDetailsViewModel>()
        .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => src.AddressCoordinates))
        .ForMember(dest => dest.FreeSpots, opt => opt.MapFrom(src => src.Capacity - src.ActiveCarsCount))
        .ForMember(dest => dest.Area, opt => opt.MapFrom(src => Enum.Parse<Area>(src.Area)))
        .ForMember(dest => dest.CanSeeRevenue, opt => opt.MapFrom(src => true))
        .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue));

        }
    }
}

