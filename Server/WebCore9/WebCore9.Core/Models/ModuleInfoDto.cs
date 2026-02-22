namespace WebCore9.Core.Models;

public sealed class ModuleInfoDto
{
    public string RouteKey { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string[] ScriptFiles { get; init; } = [];

    public string RootElementTag { get; init; } = string.Empty;

    public string LoadingText { get; init; } = string.Empty;
}
