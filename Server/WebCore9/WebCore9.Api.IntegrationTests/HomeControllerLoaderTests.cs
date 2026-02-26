using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using WebCore9.Api.IntegrationTests.TestSupport;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerLoaderTests : ApiIntegrationTestBase
{
    public HomeControllerLoaderTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoader_QuandoRichiesto_AlloraRestituisce200ConApiResponseELoaderInfo()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/loader");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderInfoDto>(response);
        data.RouteKey.Should().Be("loader");
        data.RootElementTag.Should().Be("my-angular-app");
        data.UsesWebpackChunkEntries.Should().BeTrue();
    }

    [Fact]
    public async Task GetLoaderChunks_QuandoRichiesto_AlloraRestituisce200ConChunkManifest()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/loader/chunks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderChunkManifestDto>(response);
        data.RouteKey.Should().Be("loader-chunks");
        data.LegacyView.Should().Be("Views/Home/loader.cshtml");
        data.ManifestSource.Should().Be("htmlWebpackPlugin.files.chunks");
        data.AllKnownChunkKeys.Should().Contain(["polyfills", "vendors", "module1", "module2"]);
        data.DefaultModuleChunkKey.Should().Be("module1");
    }

    [Fact]
    public async Task GetLoaderChunks_QuandoConfrontatoConHtmlPluginConfig_AlloraMetadatiChunkSonoCoerenti()
    {
        // Arrange

        // Act
        var chunksResponse = await Client.GetAsync("/api/home/loader/chunks");
        var htmlPluginResponse = await Client.GetAsync("/api/home/loader/html-plugin-config");

        // Assert
        chunksResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        htmlPluginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var chunks = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderChunkManifestDto>(chunksResponse);
        var htmlPluginConfig = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderHtmlPluginConfigDto>(htmlPluginResponse);

        htmlPluginConfig.TemplateView.Should().Be(chunks.LegacyView);
        htmlPluginConfig.OutputFilenamePattern.Should().Be(chunks.OutputFilenamePattern);
        htmlPluginConfig.SourceMapFilenamePattern.Should().Be(chunks.SourceMapFilenamePattern);
        chunks.AllKnownChunkKeys.Should().Contain(htmlPluginConfig.EntryChunkKeys);
        chunks.ModuleChunkKeys.Should().Contain(htmlPluginConfig.ModuleChunkKeys);
        chunks.SharedChunkKeys.Should().Contain(htmlPluginConfig.SharedChunkKeys);
        htmlPluginConfig.EntryChunkKeys.Should().Contain(chunks.DefaultModuleChunkKey);
        htmlPluginConfig.ModuleChunkKeys.Should().ContainSingle().Which.Should().Be("module1");
    }

    [Fact]
    public async Task GetLoaderHtmlPluginConfig_QuandoRichiesto_AlloraRestituisce200ConMetadatiHtmlPlugin()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/loader/html-plugin-config");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderHtmlPluginConfigDto>(response);
        data.RouteKey.Should().Be("loader-html-plugin-config");
        data.SourceConfigFile.Should().Be("Client/webpack-html-plugin.config.js");
        data.TemplateView.Should().Be("Views/Home/loader.cshtml");
        data.GeneratedView.Should().Be("Views/Home/Module1.cshtml");
        data.HtmlPluginInject.Should().BeFalse();
        data.EntryChunkKeys.Should().ContainInOrder("polyfills", "vendors", "module1");
        data.ModuleChunkKeys.Should().ContainSingle().Which.Should().Be("module1");
    }

    [Fact]
    public async Task GetLoaderConfigDiff_QuandoRichiesto_AlloraRestituisce200ConDifferenzeAttese()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/loader/config-diff");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<LoaderWebpackConfigDiffDto>(response);
        data.RouteKey.Should().Be("loader-config-diff");
        data.BaseConfigFile.Should().Be("Client/webpack.config.js");
        data.ComparedConfigFile.Should().Be("Client/webpack-html-plugin.config.js");
        data.BaseGeneratedView.Should().Be("Views/Home/Index.cshtml");
        data.ComparedGeneratedView.Should().Be("Views/Home/Module1.cshtml");
        data.OutputFilenamePatternMatches.Should().BeTrue();
        data.SourceMapFilenamePatternMatches.Should().BeTrue();
        data.HtmlTemplateMatches.Should().BeTrue();
        data.HtmlPluginInjectMatches.Should().BeTrue();
        data.MissingEntryChunkKeysInCompared.Should().ContainSingle().Which.Should().Be("module2");
        data.MissingModuleChunkKeysInCompared.Should().ContainSingle().Which.Should().Be("module2");
        data.SharedEntryChunkKeys.Should().Contain(["polyfills", "vendors", "module1"]);
    }

    [Fact]
    public async Task GetLoader_QuandoHomeServiceGeneraEccezione_AlloraRestituisce500ConProblemDetails()
    {
        // Arrange
        using var factory = new HomeServiceOverrideFactory(homeService =>
        {
            homeService.GetLoaderInfo().Returns(_ => throw new InvalidOperationException("boom-loader"));
        });
        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync("/api/home/loader");

        // Assert
        await AssertInternalServerErrorAsync(response, "Unable to resolve loader metadata.");
    }

    [Fact]
    public async Task GetLoaderChunks_QuandoHomeServiceGeneraEccezione_AlloraRestituisce500ConProblemDetails()
    {
        // Arrange
        using var factory = new HomeServiceOverrideFactory(homeService =>
        {
            homeService.GetLoaderChunkManifest().Returns(_ => throw new InvalidOperationException("boom-chunks"));
        });
        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync("/api/home/loader/chunks");

        // Assert
        await AssertInternalServerErrorAsync(response, "Unable to resolve loader chunk manifest.");
    }

    [Fact]
    public async Task GetLoaderHtmlPluginConfig_QuandoHomeServiceGeneraEccezione_AlloraRestituisce500ConProblemDetails()
    {
        // Arrange
        using var factory = new HomeServiceOverrideFactory(homeService =>
        {
            homeService.GetLoaderHtmlPluginConfig().Returns(_ => throw new InvalidOperationException("boom-html"));
        });
        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync("/api/home/loader/html-plugin-config");

        // Assert
        await AssertInternalServerErrorAsync(response, "Unable to resolve loader html plugin config.");
    }

    [Fact]
    public async Task GetLoaderConfigDiff_QuandoHomeServiceGeneraEccezione_AlloraRestituisce500ConProblemDetails()
    {
        // Arrange
        using var factory = new HomeServiceOverrideFactory(homeService =>
        {
            homeService.GetLoaderWebpackConfigDiff().Returns(_ => throw new InvalidOperationException("boom-diff"));
        });
        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync("/api/home/loader/config-diff");

        // Assert
        await AssertInternalServerErrorAsync(response, "Unable to resolve loader webpack config diff.");
    }

    private static async Task AssertInternalServerErrorAsync(HttpResponseMessage response, string expectedDetail)
    {
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var payload = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        payload.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        payload.Title.Should().Be("Internal server error");
        payload.Detail.Should().Be(expectedDetail);
    }

    private sealed class HomeServiceOverrideFactory : WebApplicationFactory<global::Program>
    {
        private readonly Action<IHomeService> _configureSubstitute;

        public HomeServiceOverrideFactory(Action<IHomeService> configureSubstitute)
        {
            _configureSubstitute = configureSubstitute;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var homeService = Substitute.For<IHomeService>();
                _configureSubstitute(homeService);

                services.RemoveAll<IHomeService>();
                services.AddScoped(_ => homeService);
            });
        }
    }
}
