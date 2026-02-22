using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHealthService
{
    HealthStatusDto GetStatus(string environmentName);
}
