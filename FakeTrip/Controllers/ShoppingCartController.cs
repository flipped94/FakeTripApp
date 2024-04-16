using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Helpers;
using FakeTrip.Models;
using FakeTrip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FakeTrip.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppingCartController : ControllerBase
{
    private readonly IHttpContextAccessor httpContextAccessor;

    private readonly ITouristRouteRepository touristRouteRepository;

    private readonly IMapper mapper;

    public ShoppingCartController(IHttpContextAccessor httpContextAccessor,
        ITouristRouteRepository touristRouteRepository,
        IMapper mapper)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.touristRouteRepository = touristRouteRepository;
        this.mapper = mapper;
    }

    // GET: api/ShoppingCart
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ShoppingCartDto>> GetShoppingCart()
    {
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var shoppingCartFromRepo = await touristRouteRepository.GetShoppingCartByUserIdAsync(userId);

        return Ok(mapper.Map<ShoppingCartDto>(shoppingCartFromRepo));
    }


    // POST api/ShoppingCart/items
    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
    {
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var shoppingCart = await touristRouteRepository.GetShoppingCartByUserIdAsync(userId);

        var touristRoute = await touristRouteRepository
                .GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
        if (touristRoute is null)
        {
            return NotFound("旅游路线不存在");
        }

        var lineItem = new LineItem()
        {
            TouristRouteId = addShoppingCartItemDto.TouristRouteId,
            ShoppingCartId = shoppingCart!.Id,
            OriginalPrice = touristRoute.OriginalPrice,
            DiscountPresent = touristRoute.DiscountPresent
        };

        await touristRouteRepository.AddShoppingCartItemAsync(lineItem);
        await touristRouteRepository.SaveAsync();

        return Ok(mapper.Map<ShoppingCartDto>(shoppingCart));
    }

    [Authorize]
    [HttpDelete("items/{id}")]
    public async Task<ActionResult> DeleteShoppingCartItem([FromRoute] int id)
    {
        var lineItem = await touristRouteRepository.GetShoppingCartItemByItemId(id);
        if (lineItem is null)
        {
            return NotFound("购物车商品找不到");
        }
        touristRouteRepository.DeleteShoppingCartItem(lineItem);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("items/({ids})")]
    public async Task<ActionResult> RemoveShoppingCartItems(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))]
         [FromRoute] IEnumerable<int> ids)
    {
        var lineitems = await touristRouteRepository.GetShoppingCartItemsByIdsAsync(ids);
        touristRouteRepository.DeleteShoppingCartItems(lineitems);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }
}
