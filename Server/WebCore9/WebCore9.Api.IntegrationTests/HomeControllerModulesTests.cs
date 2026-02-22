using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Api.IntegrationTests.TestSupport;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerModulesTests : ApiIntegrationTestBase
{
    public HomeControllerModulesTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetModules_QuandoRichiesto_AlloraRestituisce200ConListaModuliArricchita()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home/modules");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<IReadOnlyList<ModuleInfoDto>>(response);
        data.Should().HaveCount(2);
        data.Should().ContainSingle(m => m.RouteKey == "module1" && m.LegacyView == "Views/Home/Module1.cshtml");
        data.Should().ContainSingle(m => m.RouteKey == "module2" && m.ClientBundleName == "module2");
    }

    [Fact]
    public async Task GetModule_QuandoChiaveValida_AlloraRestituisce200EPayloadAtteso()
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
    public async Task GetModule_QuandoChiaveNonValida_AlloraRestituisce404ConProblemDetails()
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
    public async Task GetModule_QuandoChiaveConCaratteriNonConsentiti_AlloraRestituisce400ConProblemDetailsDiValidazione()
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
    public async Task GetModule_QuandoChiaveRiservataLoader_AlloraRestituisce409ConProblemDetailsDiConflitto()
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
