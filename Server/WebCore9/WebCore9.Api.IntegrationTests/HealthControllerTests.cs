using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Api.IntegrationTests.TestSupport;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HealthControllerTests : ApiIntegrationTestBase
{
    public HealthControllerTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_QuandoRichiesto_AlloraRestituisce200ConApiResponseEHealthStatus()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HealthStatusDto>(response);
        data.Status.Should().Be("Healthy");
        data.Service.Should().Be("WebCore9.Api");
        data.Environment.Should().NotBeNullOrWhiteSpace();
    }
}
