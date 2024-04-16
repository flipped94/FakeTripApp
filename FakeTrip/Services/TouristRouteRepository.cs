using FakeTrip.Databases;
using FakeTrip.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeTrip.Services;

public class TouristRouteRepository : ITouristRouteRepository
{
    private readonly AppDbContext appDbContext;

    public TouristRouteRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public void AddTouristRoute(TouristRoute touristRoute)
    {
        if (touristRoute == null)
        {
            throw new ArgumentNullException(nameof(touristRoute));
        }
        appDbContext.TouristRoutes.Add(touristRoute);
    }

    public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
    {
        if (touristRouteId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(touristRouteId));
        }
        if (touristRoutePicture == null)
        {
            throw new ArgumentNullException(nameof(touristRoutePicture));
        }
        touristRoutePicture.TouristRouteId = touristRouteId;
        appDbContext.TouristRoutePictures.Add(touristRoutePicture);
    }

    public void DeleteTouristRoute(TouristRoute touristRoute)
    {
        appDbContext.TouristRoutes.Remove(touristRoute);
    }

    public TouristRoute? GetTouristRoute(Guid id)
    {
        return appDbContext.TouristRoutes
            .Include(t => t.TouristRoutePictures)
            .FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<TouristRoutePicture> GetTouristRoutePicturesByTouristRouteId(Guid id)
    {
        return appDbContext.TouristRoutePictures
            .Where(x => x.TouristRouteId == id)
            .ToList();
    }

    public IEnumerable<TouristRoute> GetTouristRoutes(string? keyword, string? operatorType, int? raringValue)
    {
        IQueryable<TouristRoute> result = appDbContext.TouristRoutes
            .Include(t => t.TouristRoutePictures);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            result = result.Where(t => t.Title.Contains(keyword));
        }
        if (raringValue >= 0)
        {
            result = operatorType switch
            {
                "largerThan" => result.Where(t => t.Rating >= raringValue),
                "lessThan" => result.Where(t => t.Rating <= raringValue),
                _ => result.Where(t => t.Rating == raringValue),
            };
        }
        return result.ToList();
    }

    public TouristRoutePicture? GetPicture(int pictureId)
    {
        return appDbContext.TouristRoutePictures.FirstOrDefault(x => x.Id == pictureId);
    }

    public bool HasTouristRoute(Guid id)
    {
        return appDbContext.TouristRoutes.Any(x => x.Id == id);
    }

    public bool Save()
    {
        return appDbContext.SaveChanges() >= 0;
    }

    public void DeleteTouristRoutePicture(TouristRoutePicture? picture)
    {
        appDbContext.TouristRoutePictures.Remove(picture);
    }

    public IEnumerable<TouristRoute> GetTouristRoutesByIds(IEnumerable<Guid> ids)
    {
        return appDbContext.TouristRoutes.Where(t => ids.Contains(t.Id)).ToList();
    }

    public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
    {
        appDbContext.TouristRoutes.RemoveRange(touristRoutes);
    }
}
