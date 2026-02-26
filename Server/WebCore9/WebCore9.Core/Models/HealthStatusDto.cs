namespace WebCore9.Core.Models;

public sealed class HealthStatusDto
{
    public string Status { get; init; } = string.Empty;

    public string Service { get; init; } = string.Empty;

    public string Environment { get; init; } = string.Empty;

    public DateTimeOffset UtcTimestamp { get; init; }
}
