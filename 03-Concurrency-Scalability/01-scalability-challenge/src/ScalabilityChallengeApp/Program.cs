var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/process", async (HttpContext context) =>
{
    const int count = 10000;
    var tasks = Enumerable.Range(0, count).Select(SimulateWork);
    var results = await Task.WhenAll(tasks);
    await context.Response.WriteAsJsonAsync(results);
});

app.Run();

static async Task<int> SimulateWork(int n)
{
    await Task.Delay(2);
    return n * n;
}
