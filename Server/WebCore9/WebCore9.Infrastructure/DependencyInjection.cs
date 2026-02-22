using Microsoft.Extensions.DependencyInjection;
using WebCore9.Core.Abstractions;
using WebCore9.Infrastructure.Repositories;
using WebCore9.Infrastructure.Services;

namespace WebCore9.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string? heroStorageFilePath = null)
    {
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IHeroService, HeroService>();
        services.AddSingleton<IHeroRepository>(_ =>
            new JsonFileHeroRepository(heroStorageFilePath ?? Path.Combine(AppContext.BaseDirectory, "App_Data", "heroes-store.json")));

        return services;
    }
}
