using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PollingSystem.Api.Auth;
using PollingSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtTokenGenerator.GetValidationParameters();
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "Polling System API",
    endpoints = new[]
    {
        "POST /api/auth/token",
        "POST /api/polls (Admin)",
        "GET  /api/polls",
        "POST /api/polls/{id}/vote (User)",
        "GET  /api/polls/{id}/results (Analyst/Admin)"
    }
}));

app.Run();
