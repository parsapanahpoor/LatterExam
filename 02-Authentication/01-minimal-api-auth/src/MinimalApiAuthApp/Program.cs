var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string ValidUsername = "admin";
const string ValidPassword = "P@ssw0rd!";
const string ValidToken = "secure-token-12345";

app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Username != ValidUsername || request.Password != ValidPassword)
        return Results.Unauthorized();

    return Results.Ok(new LoginResponse(ValidToken));
});

app.MapGet("/protected", (HttpContext context) =>
{
    if (!TryGetBearerToken(context, out var token) || token != ValidToken)
        return Results.Unauthorized();

    return Results.Ok(new { message = "Access granted to protected resource." });
});

app.MapGet("/profile", (HttpContext context) =>
{
    if (!TryGetBearerToken(context, out var token) || token != ValidToken)
        return Results.Unauthorized();

    return Results.Ok(new { username = ValidUsername, role = "admin" });
});

app.Run();

static bool TryGetBearerToken(HttpContext context, out string token)
{
    token = string.Empty;

    if (!context.Request.Headers.TryGetValue("Authorization", out var headerValues))
        return false;

    var header = headerValues.ToString();
    const string prefix = "Bearer ";
    if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        return false;

    token = header[prefix.Length..].Trim();
    return !string.IsNullOrWhiteSpace(token);
}

record LoginRequest(string Username, string Password);
record LoginResponse(string Token);
