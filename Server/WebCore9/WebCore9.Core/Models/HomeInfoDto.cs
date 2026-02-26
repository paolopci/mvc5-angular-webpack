namespace WebCore9.Core.Models;

public sealed class HomeInfoDto
{
    public string DefaultModule { get; init; } = string.Empty;

    public string[] AvailableModules { get; init; } = [];

    public string LegacyController { get; init; } = string.Empty;
}
