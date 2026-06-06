using Microsoft.AspNetCore.Mvc;
using RestaurantOrderApi.DTOs;
using RestaurantOrderApi.Models;
using RestaurantOrderApi.Services;

namespace RestaurantOrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = await orderService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(CancellationToken ct)
        => Ok(await orderService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken ct)
    {
        var order = await orderService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken ct)
    {
        try
        {
            var order = await orderService.UpdateStatusAsync(id, request.Status, ct);
            return order is null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
