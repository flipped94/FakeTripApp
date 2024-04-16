using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;

namespace FakeTrip.Profiles;

public class TouristRoutePictureProfile : Profile
{
    public TouristRoutePictureProfile()
    {
        CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
        CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
    }
}
