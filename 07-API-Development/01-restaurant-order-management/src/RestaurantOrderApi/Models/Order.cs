namespace RestaurantOrderApi.Models;

public enum OrderStatus
{
    Pending,
    Preparing,
    Ready,
    Delivered
}

public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<string> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
