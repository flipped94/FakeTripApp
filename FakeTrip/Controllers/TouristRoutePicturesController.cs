using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;
using FakeTrip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeTrip.Controllers;

[Route("api/TouristRoutes/{touristRouteId}/pictures")]
[ApiController]
public class TouristRoutePicturesController : ControllerBase
{
    private readonly ITouristRouteRepository touristRouteRepository;
    private readonly IMapper mapper;

    public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository,
        IMapper mapper)
    {
        this.touristRouteRepository = touristRouteRepository;
        this.mapper = mapper;
    }

    // GET: api/TouristRoutes/{touristRouteId}/pictures
    [HttpGet]
    public async Task<ActionResult<IList<TouristRoutePictureDto>>> GetPictureListForTouristRoute(Guid touristRouteId)
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(touristRouteId);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        IEnumerable<TouristRoutePicture>? picturesFromrepo =
         await touristRouteRepository.GetTouristRoutePicturesByTouristRouteIdAsync(touristRouteId);
        if (picturesFromrepo == null || picturesFromrepo.Count() == 0)
        {
            return NotFound("图片不存在");
        }
        return Ok(mapper.Map<IEnumerable<TouristRoutePicture>>(picturesFromrepo));
    }

    // GET: api/TouristRoutes/{touristRouteId}/pictures/{pictureId}
    [HttpGet("{pictureId}", Name = "GetPictureById")]
    public async Task<ActionResult<TouristRoutePictureDto>> GetPicture(Guid touristRouteId, int pictureId)
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(touristRouteId);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        TouristRoutePicture? pictureFromrepo =
           await touristRouteRepository.GetPictureAsync(pictureId);
        if (pictureFromrepo == null)
        {
            return NotFound("图片不存在");
        }
        return Ok(mapper.Map<TouristRoutePicture>(pictureFromrepo));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto
        )
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(touristRouteId);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }

        var touristRoutePicture = mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
        touristRouteRepository.AddTouristRoutePicture(touristRouteId, touristRoutePicture);
        await touristRouteRepository.SaveAsync();
        var pictureToReturn = mapper.Map<TouristRoutePictureDto>(touristRoutePicture);
        return CreatedAtRoute(
            "GetPictureById",
            new
            {
                touristRouteId = touristRoutePicture.TouristRouteId,
                pictureId = touristRoutePicture.Id
            },
            pictureToReturn
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePicture([FromRoute] Guid touristRouteId, [FromRoute] int id)
    {
        var has = await touristRouteRepository.HasTouristRouteAsync(touristRouteId);
        if (!has)
        {
            return NotFound("旅游路线找不到");
        }
        var picture = await touristRouteRepository.GetPictureAsync(id);
        touristRouteRepository.DeleteTouristRoutePicture(picture);
        await touristRouteRepository.SaveAsync();

        return NoContent();
    }

}
