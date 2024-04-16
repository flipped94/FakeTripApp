using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;

namespace FakeTrip.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(
                dest => dest.State,
                opt =>
                {
                    opt.MapFrom(src => src.State.ToString());
                }
            );
    }
}
