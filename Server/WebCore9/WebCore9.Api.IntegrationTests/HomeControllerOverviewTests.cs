using System.Net;
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
}
