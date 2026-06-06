using PollingSystem.Application.Interfaces;
using PollingSystem.Application.Services;
using PollingSystem.Infrastructure.Repositories;
using PollingSystem.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace PollingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPollRepository, InMemoryPollRepository>();
        services.AddSingleton<IEncryptionService, AesEncryptionService>();
        services.AddScoped<PollService>();
        return services;
    }
}
