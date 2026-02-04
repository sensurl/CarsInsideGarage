using AutoMapper;
using CarsInsideGarage.Data.Entities;
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
                .ForMember(d => d.Area, opt => opt.MapFrom(g => g.Location.Area))
                .ForMember(d => d.AddressCoordinates, opt => opt.MapFrom(g => g.Location.AddressCoordinates))
                .ForMember(d => d.HourlyRate, opt => opt.MapFrom(g => g.GarageFee.HourlyRate))
                .ForMember(d => d.ActiveCarsCount,
                    opt => opt.MapFrom(g => g.Sessions.Count(s => s.ExitTime == null)))
                .ForMember(d => d.ActiveSessions,
                    opt => opt.MapFrom(g => g.Sessions.Where(s => s.ExitTime == null)));

            

            //  DTO <=> Entity
            CreateMap<Car, CarDto>().ReverseMap();
           
            
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<GarageFee, FeeDto>().ReverseMap();
            CreateMap<ParkingSession, SessionDto>().ReverseMap();

            // DTO <=> ViewModel
            CreateMap<CarDto, CarViewModel>().ReverseMap();
            //CreateMap<GarageDetailsDto, GarageDetailsViewModel>();



        }
    }
}

