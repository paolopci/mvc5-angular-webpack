namespace WebCore9.Core.Models;

public sealed class LoaderHtmlPluginConfigDto
{
    public string RouteKey { get; init; } = string.Empty;

    public string SourceConfigFile { get; init; } = string.Empty;

    public string TemplateView { get; init; } = string.Empty;

    public string GeneratedView { get; init; } = string.Empty;

    public string OutputPath { get; init; } = string.Empty;

    public string OutputFilenamePattern { get; init; } = string.Empty;

    public string SourceMapFilenamePattern { get; init; } = string.Empty;

    public bool HtmlPluginInject { get; init; }

    public string[] EntryChunkKeys { get; init; } = [];

    public string[] SharedChunkKeys { get; init; } = [];

    public string[] ModuleChunkKeys { get; init; } = [];
}
