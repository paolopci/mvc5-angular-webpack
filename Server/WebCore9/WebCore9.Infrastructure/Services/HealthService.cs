using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HealthService : IHealthService
{
    public HealthStatusDto GetStatus(string environmentName)
    {
        return new HealthStatusDto
        {
            Status = "Healthy",
            Service = "WebCore9.Api",
            Environment = environmentName,
            UtcTimestamp = DateTimeOffset.UtcNow
        };
    }
}
