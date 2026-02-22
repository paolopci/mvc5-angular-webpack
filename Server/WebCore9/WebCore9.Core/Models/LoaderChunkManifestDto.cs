namespace WebCore9.Core.Models;

public sealed class LoaderChunkManifestDto
{
    public string RouteKey { get; init; } = string.Empty;

    public string LegacyView { get; init; } = string.Empty;

    public string ManifestSource { get; init; } = string.Empty;

    public string ChunkEntryExpression { get; init; } = string.Empty;

    public string OutputFilenamePattern { get; init; } = string.Empty;

    public string SourceMapFilenamePattern { get; init; } = string.Empty;

    public string[] SharedChunkKeys { get; init; } = [];

    public string[] ModuleChunkKeys { get; init; } = [];

    public string[] AllKnownChunkKeys { get; init; } = [];

    public string DefaultModuleChunkKey { get; init; } = string.Empty;
}
