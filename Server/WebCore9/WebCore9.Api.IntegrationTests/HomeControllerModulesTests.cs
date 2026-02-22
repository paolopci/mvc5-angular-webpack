using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Core.Models;
using WebCore9.Api.IntegrationTests.TestSupport;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerModulesTests : ApiIntegrationTestBase
{
    public HomeControllerModulesTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoader_Restituisce200ConApiResponseELoaderInfo()
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
    public async Task GetHealth_Restituisce200ConApiResponseEHealthStatus()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HealthStatusDto>(response);
        data.Status.Should().Be("Healthy");
        data.Service.Should().Be("WebCore9.Api");
    }

    [Fact]
    public async Task GetModule_ConChiaveValida_Restituisce200EPayloadAtteso()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/modules/module1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<ModuleInfoDto>(response);
        data.RouteKey.Should().Be("module1");
        data.Title.Should().Be("Module 1");
        data.RootElementTag.Should().Be("my-angular-app");
        data.LegacyView.Should().Be("Views/Home/Module1.cshtml");
        data.ClientBundleName.Should().Be("module1");
        data.UsesPrebuiltNg2Bundles.Should().BeTrue();
    }

    [Fact]
    public async Task GetModule_ConChiaveNonValida_Restituisce404ConProblemDetails()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/modules/invalid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var payload = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        payload.Status.Should().Be((int)HttpStatusCode.NotFound);
        payload.Title.Should().Be("Module key not found");
        payload.Detail.Should().Contain("invalid");
    }

    [Fact]
    public async Task GetModule_ConChiaveConCaratteriNonConsentiti_Restituisce400ConProblemDetailsDiValidazione()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/modules/module.1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var payload = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        payload.Status.Should().Be((int)HttpStatusCode.BadRequest);
        payload.Title.Should().Be("Validation failed");
        payload.Detail.Should().Contain("must not contain");
    }

    [Fact]
    public async Task GetModule_ConChiaveRiservataLoader_Restituisce409ConProblemDetailsDiConflitto()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/modules/loader");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var payload = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        payload.Status.Should().Be((int)HttpStatusCode.Conflict);
        payload.Title.Should().Be("Reserved module key");
        payload.Detail.Should().Contain("/api/home/loader");
    }
}
