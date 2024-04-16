using AutoMapper;
using FakeTrip.Constants;
using FakeTrip.Dtos;
using FakeTrip.Helpers;
using FakeTrip.Models;
using FakeTrip.ResourceParameters;
using FakeTrip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FakeTrip.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TouristRoutesController : ControllerBase
{
    private readonly ITouristRouteRepository touristRouteRepository;
    private readonly IMapper mapper;

    public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
    {
        this.touristRouteRepository = touristRouteRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<TouristRouteDto>> GetTouristRoutes([FromQuery] TouristRoutesResourceParameters parameters)
    {

        var touristRoutesFromRepo = await touristRouteRepository.GetTouristRoutesAsync(
            parameters.Keyword,
            parameters.RatingOperator,
            parameters.RatingValue,
            parameters.PageSize,
            parameters.PageNumber);
        if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
        {
            return NotFound("没有旅游路线");
        }
        var touristRoutesDto = mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
        return Ok(touristRoutesDto);
    }

    [HttpGet("{id}", Name = "GetTouristRouteById")]
    public async Task<ActionResult<TouristRouteDto>> GetTouristRouteById(Guid id)
    {
        TouristRoute? touristRouteFromRepo = await touristRouteRepository.GetTouristRouteAsync(id);
        if (touristRouteFromRepo == null)
        {
            return NotFound("旅游路线不存在");
        }
        var output = mapper.Map<TouristRouteDto>(touristRouteFromRepo);
        return Ok(output);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
    {
        var touristRoute = mapper.Map<TouristRoute>(touristRouteForCreationDto);
        touristRouteRepository.AddTouristRoute(touristRoute);
        var touristRouteToReture = mapper.Map<TouristRouteDto>(touristRoute);
        await touristRouteRepository.SaveAsync();
        return CreatedAtRoute(
                "GetTouristRouteById",
                new { id = touristRouteToReture.Id },
                touristRouteToReture
            );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTouristRoute([FromRoute] Guid id,
        [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(id);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        var touristRouteFromRepo = await touristRouteRepository.GetTouristRouteAsync(id);
        mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid id,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(id);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        var touristRouteFromRepo = await touristRouteRepository.GetTouristRouteAsync(id);
        var touristRouteToPatch = mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
        patchDocument.ApplyTo(touristRouteToPatch, ModelState);

        if (!TryValidateModel(touristRouteToPatch))
        {
            return ValidationProblem(ModelState);
        }

        mapper.Map(touristRouteToPatch, touristRouteFromRepo);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTouristRoute([FromRoute] Guid id)
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(id);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        var touristRoute = await touristRouteRepository.GetTouristRouteAsync(id);
        touristRouteRepository.DeleteTouristRoute(touristRoute!);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("({ids})")]
    public async Task<ActionResult> DeleteByIDs(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> ids)
    {
        if (ids == null)
        {
            return BadRequest();
        }
        var touristRoutesFromRepo = await touristRouteRepository.GetTouristRoutesByIdsAsync(ids);
        touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
        await touristRouteRepository.SaveAsync();
        return NoContent();
    }
}
