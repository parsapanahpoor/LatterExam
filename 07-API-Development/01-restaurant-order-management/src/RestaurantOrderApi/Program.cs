using System.Text.Json.Serialization;
using RestaurantOrderApi.Models;
using RestaurantOrderApi.Repositories;
using RestaurantOrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "Restaurant Order Management API",
    statuses = Enum.GetNames<OrderStatus>(),
    endpoints = new[]
    {
        "POST   /api/orders",
        "GET    /api/orders",
        "GET    /api/orders/{id}",
        "PATCH  /api/orders/{id}/status"
    }
}));

app.Run();
