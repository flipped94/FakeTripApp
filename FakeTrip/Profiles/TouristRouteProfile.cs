using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;

namespace FakeTrip.Profiles;

public class TouristRouteProfile : Profile
{
    public TouristRouteProfile()
    {
        CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember(
                    dest => dest.TravelDays,
                    opt => opt.MapFrom(src => src.TravelDays.ToString())
                )
                .ForMember(
                    dest => dest.TripType,
                    opt => opt.MapFrom(src => src.TripType.ToString())
                )
                .ForMember(
                    dest => dest.DepartureCity,
                    opt => opt.MapFrom(src => src.DepartureCity.ToString())
                );

        CreateMap<TouristRouteForCreationDto, TouristRoute>()
            .ForMember(
            dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid())
                    )
            .ForMember(
             dest => dest.CreateTime,
            opt => opt.MapFrom(src => DateTime.UtcNow)
            );

        CreateMap<TouristRouteForUpdateDto, TouristRoute>();
    }
}
