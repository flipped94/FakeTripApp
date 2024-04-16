using FakeTrip.Models;

namespace FakeTrip.Services;

public interface ITouristRouteRepository
{
    IEnumerable<TouristRoute> GetTouristRoutes(string? keyword, string? operatorType, int? raringValue);

    TouristRoute? GetTouristRoute(Guid id);

    bool HasTouristRoute(Guid id);

    IEnumerable<TouristRoutePicture> GetTouristRoutePicturesByTouristRouteId(Guid id);

    TouristRoutePicture? GetPicture(int pictureId);

    void AddTouristRoute(TouristRoute touristRoute);

    void DeleteTouristRoute(TouristRoute touristRoute);

    bool Save();

    void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);

    void DeleteTouristRoutePicture(TouristRoutePicture? picture);

    IEnumerable<TouristRoute> GetTouristRoutesByIds(IEnumerable<Guid> ids);

    void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
}
