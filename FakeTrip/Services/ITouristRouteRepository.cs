﻿using FakeTrip.Helpers;
using FakeTrip.Models;

namespace FakeTrip.Services;

public interface ITouristRouteRepository
{
    Task<Pagination<TouristRoute>> GetTouristRoutesAsync(
        string? keyword,
        string? operatorType,
        int? raringValue,
        int pageSize,
        int pageNumber);

    Task<TouristRoute?> GetTouristRouteAsync(Guid id);

    Task<bool> HasTouristRouteAsync(Guid id);

    Task<IEnumerable<TouristRoutePicture>> GetTouristRoutePicturesByTouristRouteIdAsync(Guid id);

    Task<TouristRoutePicture?> GetPictureAsync(int pictureId);

    void AddTouristRoute(TouristRoute touristRoute);

    void DeleteTouristRoute(TouristRoute touristRoute);

    Task<bool> SaveAsync();

    void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);

    void DeleteTouristRoutePicture(TouristRoutePicture? picture);

    Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> ids);

    void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

    Task CreateShoppingCartAsync(ShoppingCart shoppingCart);

    Task<ShoppingCart?> GetShoppingCartByUserIdAsync(string userId);

    Task AddShoppingCartItemAsync(LineItem lineItem);

    Task<LineItem?> GetShoppingCartItemByItemId(int itemId);

    void DeleteShoppingCartItem(LineItem lineItem);

    Task<IEnumerable<LineItem>> GetShoppingCartItemsByIdsAsync(IEnumerable<int> ids);

    void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);

    Task AddOrderAsync(Order order);

    Task<Pagination<Order>> GetOrdersByUserIdAsync(string userId, int pageSize, int pageNumber);

    public Task<Order?> GetOrderByIdAsync(Guid orderId);
}
