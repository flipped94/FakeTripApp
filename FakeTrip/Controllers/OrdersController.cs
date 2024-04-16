using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FakeTrip.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IHttpContextAccessor httpContextAccessor;

    private readonly ITouristRouteRepository touristRouteRepository;

    private readonly IMapper mapper;

    public OrdersController(
        IHttpContextAccessor httpContextAccessor,
        ITouristRouteRepository touristRouteRepository,
        IMapper mapper)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.touristRouteRepository = touristRouteRepository;
        this.mapper = mapper;
    }



    // GET: api/<OrdersController>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userId = GetUserId();
        var orders = await touristRouteRepository.GetOrdersByUserIdAsync(userId);
        return Ok(mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("{orderId}")]
    [Authorize]
    public async Task<IActionResult> GerOrderById([FromRoute] Guid orderId)
    {
        // 1. 获得当前用户
        var userId = GetUserId();

        var order = await touristRouteRepository.GetOrderByIdAsync(orderId);

        return Ok(mapper.Map<OrderDto>(order));
    }

    private string GetUserId()
    {
        return httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
