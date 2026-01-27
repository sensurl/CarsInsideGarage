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
            // Entity to DTO mappings
            CreateMap<Car, CarDto>();
            CreateMap<Garage, GarageDto>();
            CreateMap<Location, LocationDto>();
            CreateMap<GarageFee, FeeDto>();
            CreateMap<ParkingSession, SessionDto>();


            // DtO to Entity mappings
            CreateMap<CarDto, Car>();


            // DTO to ViewModels mappings
            CreateMap<CarDto, CarViewModel>();

        }
    }
}

