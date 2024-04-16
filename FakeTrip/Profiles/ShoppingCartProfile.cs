using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;

namespace FakeTrip.Profiles;

public class ShoppingCartProfile : Profile
{
    public ShoppingCartProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartDto>();
        CreateMap<LineItem, LineItemDto>();
    }
}
