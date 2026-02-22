using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HomeService : IHomeService
{
    private static readonly string[] SharedScripts =
    [
        "~/Scripts/ng2/polyfills.js",
        "~/Scripts/ng2/vendors.js"
    ];

    private static readonly string[] SharedChunkKeys = ["polyfills", "vendors"];
    private static readonly string[] ModuleChunkKeys = ["module1", "module2"];

    public HomeInfoDto GetHomeInfo()
    {
        return new HomeInfoDto
        {
            DefaultModule = "module1",
            AvailableModules = ["module1", "module2"],
            LegacyController = "Home"
        };
    }

    public IReadOnlyList<ModuleInfoDto> GetModules()
    {
        return
        [
            CreateModule1Info(),
            CreateModule2Info()
        ];
    }

    public ModuleInfoDto? GetModuleInfo(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        return key.Trim().ToLowerInvariant() switch
        {
            "module1" => CreateModule1Info(),
            "module2" => CreateModule2Info(),
            _ => null
        };
    }

    public LoaderInfoDto GetLoaderInfo()
    {
        return new LoaderInfoDto
        {
            RouteKey = "loader",
            Title = "Module 1",
            RootElementTag = "my-angular-app",
            LoadingText = "Loading....",
            UsesWebpackChunkEntries = true,
            ChunkEntryExpression = "htmlWebpackPlugin.files.chunks[chunk].entry",
            LegacyView = "Views/Home/loader.cshtml"
        };
    }

    public LoaderChunkManifestDto GetLoaderChunkManifest()
    {
        return new LoaderChunkManifestDto
        {
            RouteKey = "loader-chunks",
            LegacyView = "Views/Home/loader.cshtml",
            ManifestSource = "htmlWebpackPlugin.files.chunks",
            ChunkEntryExpression = "htmlWebpackPlugin.files.chunks[chunk].entry",
            OutputFilenamePattern = "[name].[hash].js",
            SourceMapFilenamePattern = "[name].[hash].js.map",
            SharedChunkKeys = [.. SharedChunkKeys],
            ModuleChunkKeys = [.. ModuleChunkKeys],
            AllKnownChunkKeys = [.. SharedChunkKeys, .. ModuleChunkKeys],
            DefaultModuleChunkKey = "module1"
        };
    }

    public LoaderHtmlPluginConfigDto GetLoaderHtmlPluginConfig()
    {
        return new LoaderHtmlPluginConfigDto
        {
            RouteKey = "loader-html-plugin-config",
            SourceConfigFile = "Client/webpack-html-plugin.config.js",
            TemplateView = "Views/Home/loader.cshtml",
            GeneratedView = "Views/Home/Module1.cshtml",
            OutputPath = "Server/WebApplication/Scripts/ng2",
            OutputFilenamePattern = "[name].[hash].js",
            SourceMapFilenamePattern = "[name].[hash].js.map",
            HtmlPluginInject = false,
            EntryChunkKeys = ["polyfills", "vendors", "module1"],
            SharedChunkKeys = ["vendors", "polyfills"],
            ModuleChunkKeys = ["module1"]
        };
    }

    public LoaderWebpackConfigDiffDto GetLoaderWebpackConfigDiff()
    {
        var chunkManifest = GetLoaderChunkManifest();
        var htmlPluginConfig = GetLoaderHtmlPluginConfig();
        var baseHomeInfo = GetHomeInfo();

        var baseEntryChunkKeys = chunkManifest.AllKnownChunkKeys;
        var comparedEntryChunkKeys = htmlPluginConfig.EntryChunkKeys;
        var sharedEntryChunkKeys = baseEntryChunkKeys.Intersect(comparedEntryChunkKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var missingEntryChunkKeysInCompared = baseEntryChunkKeys.Except(comparedEntryChunkKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var additionalEntryChunkKeysInCompared = comparedEntryChunkKeys.Except(baseEntryChunkKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var missingModuleChunkKeysInCompared = chunkManifest.ModuleChunkKeys
            .Except(htmlPluginConfig.ModuleChunkKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var additionalModuleChunkKeysInCompared = htmlPluginConfig.ModuleChunkKeys
            .Except(chunkManifest.ModuleChunkKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new LoaderWebpackConfigDiffDto
        {
            RouteKey = "loader-config-diff",
            BaseConfigFile = "Client/webpack.config.js",
            ComparedConfigFile = htmlPluginConfig.SourceConfigFile,
            BaseGeneratedView = "Views/Home/Index.cshtml",
            ComparedGeneratedView = htmlPluginConfig.GeneratedView,
            TemplateView = htmlPluginConfig.TemplateView,
            OutputFilenamePatternMatches = chunkManifest.OutputFilenamePattern == htmlPluginConfig.OutputFilenamePattern,
            SourceMapFilenamePatternMatches = chunkManifest.SourceMapFilenamePattern == htmlPluginConfig.SourceMapFilenamePattern,
            HtmlTemplateMatches = string.Equals(htmlPluginConfig.TemplateView, chunkManifest.LegacyView, StringComparison.Ordinal),
            HtmlPluginInjectMatches = htmlPluginConfig.HtmlPluginInject == false,
            BaseEntryChunkKeys = [.. baseEntryChunkKeys],
            ComparedEntryChunkKeys = [.. comparedEntryChunkKeys],
            SharedEntryChunkKeys = sharedEntryChunkKeys,
            MissingEntryChunkKeysInCompared = missingEntryChunkKeysInCompared,
            AdditionalEntryChunkKeysInCompared = additionalEntryChunkKeysInCompared,
            BaseModuleChunkKeys = [.. chunkManifest.ModuleChunkKeys],
            ComparedModuleChunkKeys = [.. htmlPluginConfig.ModuleChunkKeys],
            MissingModuleChunkKeysInCompared = missingModuleChunkKeysInCompared,
            AdditionalModuleChunkKeysInCompared = additionalModuleChunkKeysInCompared,
            Notes =
            [
                "Il config html-plugin genera solo la view Module1.cshtml.",
                $"Il modulo di default legacy resta '{baseHomeInfo.DefaultModule}'.",
                "module2 e' escluso dal config html-plugin dedicato."
            ]
        };
    }

    private static ModuleInfoDto CreateModule1Info()
    {
        return new ModuleInfoDto
        {
            RouteKey = "module1",
            Title = "Module 1",
            LegacyView = "Views/Home/Module1.cshtml",
            ClientBundleName = "module1",
            ScriptFiles = [.. SharedScripts, "~/Scripts/ng2/module1.js"],
            RootElementTag = "my-angular-app",
            LoadingText = "Loading....",
            UsesPrebuiltNg2Bundles = true
        };
    }

    private static ModuleInfoDto CreateModule2Info()
    {
        return new ModuleInfoDto
        {
            RouteKey = "module2",
            Title = "Module 2",
            LegacyView = "Views/Home/Module2.cshtml",
            ClientBundleName = "module2",
            ScriptFiles = [.. SharedScripts, "~/Scripts/ng2/module2.js"],
            RootElementTag = "tour-of-heroes",
            LoadingText = "Loading....",
            UsesPrebuiltNg2Bundles = true
        };
    }
}
