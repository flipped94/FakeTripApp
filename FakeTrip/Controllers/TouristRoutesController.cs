using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Helpers;
using FakeTrip.Models;
using FakeTrip.ResourceParameters;
using FakeTrip.Services;
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
    public ActionResult<TouristRouteDto> GetTouristRoutes([FromQuery] TouristRoutesResourceParameters parameters)
    {

        var touristRoutesFromRepo = touristRouteRepository
            .GetTouristRoutes(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);
        if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
        {
            return NotFound("没有旅游路线");
        }
        var touristRoutesDto = mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
        return Ok(touristRoutesDto);
    }

    [HttpGet("{id}", Name = "GetTouristRouteById")]
    public ActionResult<TouristRouteDto> GetTouristRouteById(Guid id)
    {
        TouristRoute? touristRouteFromRepo = touristRouteRepository.GetTouristRoute(id);
        if (touristRouteFromRepo == null)
        {
            return NotFound("旅游路线不存在");
        }
        var output = mapper.Map<TouristRouteDto>(touristRouteFromRepo);
        return Ok(output);
    }

    [HttpPost]
    public ActionResult CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
    {
        var touristRoute = mapper.Map<TouristRoute>(touristRouteForCreationDto);
        touristRouteRepository.AddTouristRoute(touristRoute);
        var touristRouteToReture = mapper.Map<TouristRouteDto>(touristRoute);
        touristRouteRepository.Save();
        return CreatedAtRoute(
                "GetTouristRouteById",
                new { id = touristRouteToReture.Id },
                touristRouteToReture
            );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateTouristRoute([FromRoute] Guid id,
        [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
    {
        if (!touristRouteRepository.HasTouristRoute(id))
        {
            return NotFound("旅游路线找不到");
        }
        var touristRouteFromRepo = touristRouteRepository.GetTouristRoute(id);
        mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
        touristRouteRepository.Save();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateTouristRoute(
            [FromRoute] Guid id,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
    {
        if (!touristRouteRepository.HasTouristRoute(id))
        {
            return NotFound("旅游路线找不到");
        }
        var touristRouteFromRepo = touristRouteRepository.GetTouristRoute(id);
        var touristRouteToPatch = mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
        patchDocument.ApplyTo(touristRouteToPatch, ModelState);

        if (!TryValidateModel(touristRouteToPatch))
        {
            return ValidationProblem(ModelState);
        }

        mapper.Map(touristRouteToPatch, touristRouteFromRepo);
        touristRouteRepository.Save();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTouristRoute([FromRoute] Guid id)
    {
        if (!touristRouteRepository.HasTouristRoute(id))
        {
            return NotFound("旅游路线找不到");
        }
        var touristRoute = touristRouteRepository.GetTouristRoute(id);
        touristRouteRepository.DeleteTouristRoute(touristRoute!);
        touristRouteRepository.Save();
        return NoContent();
    }

    [HttpDelete("({ids})")]
    public ActionResult DeleteByIDs(
        [ModelBinder(BinderType =typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> ids)
    {
        if (ids == null)
        {
            return BadRequest();
        }
        var touristRoutesFromRepo = touristRouteRepository.GetTouristRoutesByIds(ids);
        touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
        touristRouteRepository.Save();
        return NoContent();
    }
}
