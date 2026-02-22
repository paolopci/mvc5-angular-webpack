using Microsoft.Extensions.DependencyInjection;
using WebCore9.Core.Abstractions;
using WebCore9.Infrastructure.Services;

namespace WebCore9.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IHeroService, HeroService>();

        return services;
    }
}
