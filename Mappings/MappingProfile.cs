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
            //  DTO <=> Entity
            CreateMap<Car, CarDto>().ReverseMap();
            CreateMap<Garage, GarageDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<GarageFee, FeeDto>().ReverseMap();
            CreateMap<ParkingSession, SessionDto>().ReverseMap();

            // DTO <=> ViewModel
            CreateMap<CarDto, CarViewModel>().ReverseMap();

        }
    }
}

