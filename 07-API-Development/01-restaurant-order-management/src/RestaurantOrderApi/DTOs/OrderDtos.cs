using RestaurantOrderApi.Models;

namespace RestaurantOrderApi.DTOs;

public record CreateOrderRequest(string CustomerName, List<string> Items, decimal TotalAmount);

public record UpdateOrderStatusRequest(OrderStatus Status);

public record OrderResponse(
    Guid Id,
    string CustomerName,
    List<string> Items,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
