using DependencyInjectionDemo.Interfaces;
using DependencyInjectionDemo.Repositories;
using DependencyInjectionDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // ✅ Lifetime صحیح:
        // Repository: Transient — هر resolve یک instance جدید
        // Service: Transient — هر سرویس Repository جدید دریافت می‌کند
        //
        // ❌ اشتباه قبلی (comment شده):
        // services.AddSingleton<IRepository, Repository>();
        // services.AddSingleton<IService, Service>();

        services.AddTransient<IRepository, Repository>();
        services.AddTransient<IService, Service>();
    })
    .Build();

Console.WriteLine("=== نمایش Lifetime صحیح DI ===\n");

using var scope1 = host.Services.CreateScope();
var service1a = scope1.ServiceProvider.GetRequiredService<IService>();
var service1b = scope1.ServiceProvider.GetRequiredService<IService>();

Console.WriteLine($"Scope 1 - Service A: {service1a.Process()}");
Console.WriteLine($"Scope 1 - Service B: {service1b.Process()}");
Console.WriteLine($"Repository مشترک؟ {service1a.RepositoryInstanceId == service1b.RepositoryInstanceId}\n");

using var scope2 = host.Services.CreateScope();
var service2 = scope2.ServiceProvider.GetRequiredService<IService>();
Console.WriteLine($"Scope 2 - Service: {service2.Process()}");

Console.WriteLine("\n✅ هر Service باید RepositoryInstanceId متفاوت داشته باشد (Transient).");
