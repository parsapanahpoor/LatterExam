using System.Collections.Concurrent;
using RestaurantOrderApi.Models;

namespace RestaurantOrderApi.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<Order> AddAsync(Order order, CancellationToken ct = default)
    {
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Order>>(_orders.Values.OrderByDescending(o => o.CreatedAt).ToList());

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_orders.TryGetValue(id, out var order) ? order : null);

    public Task<Order?> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        if (!_orders.TryGetValue(id, out var order))
            return Task.FromResult<Order?>(null);

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult<Order?>(order);
    }
}
