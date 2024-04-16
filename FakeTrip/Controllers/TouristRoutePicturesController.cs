using AutoMapper;
using FakeTrip.Dtos;
using FakeTrip.Models;
using FakeTrip.Services;
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
    public ActionResult<IList<TouristRoutePictureDto>> GetPictureListForTouristRoute(Guid touristRouteId)
    {
        if (!touristRouteRepository.HasTouristRoute(touristRouteId))
        {
            return NotFound("旅游线路不存在");
        }
        IEnumerable<TouristRoutePicture>? picturesFromrepo =
            touristRouteRepository.GetTouristRoutePicturesByTouristRouteId(touristRouteId);
        if (picturesFromrepo == null || picturesFromrepo.Count() == 0)
        {
            return NotFound("图片不存在");
        }
        return Ok(mapper.Map<IEnumerable<TouristRoutePicture>>(picturesFromrepo));
    }

    // GET: api/TouristRoutes/{touristRouteId}/pictures/{pictureId}
    [HttpGet("{pictureId}", Name = "GetPictureById")]
    public ActionResult<TouristRoutePictureDto> GetPicture(Guid touristRouteId, int pictureId)
    {
        if (!touristRouteRepository.HasTouristRoute(touristRouteId))
        {
            return NotFound("旅游线路不存在");
        }
        TouristRoutePicture? pictureFromrepo =
            touristRouteRepository.GetPicture(pictureId);
        if (pictureFromrepo == null)
        {
            return NotFound("图片不存在");
        }
        return Ok(mapper.Map<TouristRoutePicture>(pictureFromrepo));
    }

    [HttpPost]
    public ActionResult CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto
        )
    {
        if (!touristRouteRepository.HasTouristRoute(touristRouteId))
        {
            return NotFound("旅游线路不存在");
        }

        var touristRoutePicture = mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
        touristRouteRepository.AddTouristRoutePicture(touristRouteId, touristRoutePicture);
        touristRouteRepository.Save();
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

    [HttpDelete("{id}")]
    public ActionResult DeletePicture([FromRoute] Guid touristRouteId, [FromRoute]int id)
    {
        if (!touristRouteRepository.HasTouristRoute(touristRouteId))
        {
            return NotFound("旅游线路不存在");
        }
        var picture = touristRouteRepository.GetPicture(id);
        touristRouteRepository.DeleteTouristRoutePicture(picture);
        touristRouteRepository.Save();

        return NoContent();
    }

}
