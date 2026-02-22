using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Api.IntegrationTests.TestSupport;
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
}
