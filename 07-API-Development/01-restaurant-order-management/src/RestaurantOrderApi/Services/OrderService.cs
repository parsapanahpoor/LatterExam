using RestaurantOrderApi.DTOs;
using RestaurantOrderApi.Models;
using RestaurantOrderApi.Repositories;

namespace RestaurantOrderApi.Services;

public class OrderService(IOrderRepository repository)
{
    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Preparing],
        [OrderStatus.Preparing] = [OrderStatus.Ready],
        [OrderStatus.Ready] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = []
    };

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("نام مشتری الزامی است.");

        if (request.Items.Count == 0)
            throw new ArgumentException("حداقل یک آیتم لازم است.");

        if (request.TotalAmount <= 0)
            throw new ArgumentException("مبلغ باید بزرگ‌تر از صفر باشد.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            Items = request.Items,
            TotalAmount = request.TotalAmount
        };

        await repository.AddAsync(order, ct);
        return MapToResponse(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await repository.GetAllAsync(ct);
        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(id, ct);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderResponse?> UpdateStatusAsync(Guid id, OrderStatus newStatus, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(id, ct);
        if (order is null)
            return null;

        if (!ValidTransitions.TryGetValue(order.Status, out var allowed) || !allowed.Contains(newStatus))
            throw new InvalidOperationException(
                $"انتقال از {order.Status} به {newStatus} مجاز نیست.");

        var updated = await repository.UpdateStatusAsync(id, newStatus, ct);
        return updated is null ? null : MapToResponse(updated);
    }

    private static OrderResponse MapToResponse(Order order) =>
        new(order.Id, order.CustomerName, order.Items, order.TotalAmount,
            order.Status, order.CreatedAt, order.UpdatedAt);
}
