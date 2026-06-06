var app = WebApplication.CreateBuilder(args).Build();

var sync = new object();
var numbers = new List<int>();

app.MapPost("/add", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    if (form.TryGetValue("number", out var numberStr) && int.TryParse(numberStr, out var number))
    {
        lock (sync)
        {
            numbers.Add(number);
        }

        await context.Response.WriteAsync("Added");
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Invalid number");
    }
});

app.MapGet("/sum", () =>
{
    lock (sync)
    {
        return numbers.Sum();
    }
});

app.Run();
