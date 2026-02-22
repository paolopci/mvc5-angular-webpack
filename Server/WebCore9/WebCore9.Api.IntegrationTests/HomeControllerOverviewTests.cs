using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Api.IntegrationTests.TestSupport;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerOverviewTests : ApiIntegrationTestBase
{
    public HomeControllerOverviewTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_QuandoRichiesto_AlloraRestituisce200ConHomeInfoWrappato()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HomeInfoDto>(response);
        data.DefaultModule.Should().Be("module1");
        data.AvailableModules.Should().Contain(["module1", "module2"]);
        data.LegacyController.Should().Be("Home");
    }

    [Fact]
    public async Task Get_QuandoRichiesto_AlloraRestituisceShapeJsonCompatibileConWrapperECamelCase()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/home");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;

        root.TryGetProperty("success", out var success).Should().BeTrue();
        success.GetBoolean().Should().BeTrue();

        root.TryGetProperty("data", out var data).Should().BeTrue();
        data.TryGetProperty("defaultModule", out var defaultModule).Should().BeTrue();
        defaultModule.GetString().Should().Be("module1");
        data.TryGetProperty("availableModules", out _).Should().BeTrue();
        data.TryGetProperty("legacyController", out var legacyController).Should().BeTrue();
        legacyController.GetString().Should().Be("Home");

        root.TryGetProperty("Success", out _).Should().BeFalse();
        root.TryGetProperty("Data", out _).Should().BeFalse();
    }
}
