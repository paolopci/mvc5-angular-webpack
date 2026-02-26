namespace WebCore9.Core.Models;

public sealed class LoaderWebpackConfigDiffDto
{
    public string RouteKey { get; init; } = string.Empty;

    public string BaseConfigFile { get; init; } = string.Empty;

    public string ComparedConfigFile { get; init; } = string.Empty;

    public string BaseGeneratedView { get; init; } = string.Empty;

    public string ComparedGeneratedView { get; init; } = string.Empty;

    public string TemplateView { get; init; } = string.Empty;

    public bool OutputFilenamePatternMatches { get; init; }

    public bool SourceMapFilenamePatternMatches { get; init; }

    public bool HtmlTemplateMatches { get; init; }

    public bool HtmlPluginInjectMatches { get; init; }

    public string[] BaseEntryChunkKeys { get; init; } = [];

    public string[] ComparedEntryChunkKeys { get; init; } = [];

    public string[] SharedEntryChunkKeys { get; init; } = [];

    public string[] MissingEntryChunkKeysInCompared { get; init; } = [];

    public string[] AdditionalEntryChunkKeysInCompared { get; init; } = [];

    public string[] BaseModuleChunkKeys { get; init; } = [];

    public string[] ComparedModuleChunkKeys { get; init; } = [];

    public string[] MissingModuleChunkKeysInCompared { get; init; } = [];

    public string[] AdditionalModuleChunkKeysInCompared { get; init; } = [];

    public string[] Notes { get; init; } = [];
}
