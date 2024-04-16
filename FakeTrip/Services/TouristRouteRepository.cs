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

    public async Task<TouristRoute?> GetTouristRouteAsync(Guid id)
    {
        return await appDbContext.TouristRoutes
            .Include(t => t.TouristRoutePictures)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<TouristRoutePicture>> GetTouristRoutePicturesByTouristRouteIdAsync(Guid id)
    {
        return await appDbContext.TouristRoutePictures
            .Where(x => x.TouristRouteId == id)
            .ToListAsync();
    }

    public async Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string? keyword, string? operatorType, int? raringValue)
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
        return await result.ToListAsync();
    }

    public async Task<TouristRoutePicture?> GetPictureAsync(int pictureId)
    {
        return await appDbContext.TouristRoutePictures.FirstOrDefaultAsync(x => x.Id == pictureId);
    }

    public async Task<bool> HasTouristRouteAsync(Guid id)
    {
        return await appDbContext.TouristRoutes.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> SaveAsync()
    {
        return await appDbContext.SaveChangesAsync() >= 0;
    }

    public void DeleteTouristRoutePicture(TouristRoutePicture? picture)
    {
        appDbContext.TouristRoutePictures.Remove(picture);
    }

    public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> ids)
    {
        return await appDbContext.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
    }

    public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
    {
        appDbContext.TouristRoutes.RemoveRange(touristRoutes);
    }
}
