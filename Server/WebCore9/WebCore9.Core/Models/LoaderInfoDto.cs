namespace WebCore9.Core.Models;

public sealed class LoaderInfoDto
{
    public string RouteKey { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string RootElementTag { get; init; } = string.Empty;

    public string LoadingText { get; init; } = string.Empty;

    public bool UsesWebpackChunkEntries { get; init; }

    public string ChunkEntryExpression { get; init; } = string.Empty;

    public string LegacyView { get; init; } = string.Empty;
}
