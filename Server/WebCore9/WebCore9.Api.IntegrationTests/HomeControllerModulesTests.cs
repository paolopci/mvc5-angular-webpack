using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerModulesTests : IClassFixture<WebApplicationFactory<global::Program>>
{
    private readonly HttpClient _client;

    public HomeControllerModulesTests(WebApplicationFactory<global::Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetModule_ConChiaveValida_Restituisce200EPayloadAtteso()
    {
        var response = await _client.GetAsync("/api/home/modules/module1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ModuleResponse>();
        Assert.NotNull(payload);
        Assert.Equal("module1", payload!.RouteKey);
        Assert.Equal("Module 1", payload.Title);
        Assert.Equal("my-angular-app", payload.RootElementTag);
    }

    [Fact]
    public async Task GetModule_ConChiaveNonValida_Restituisce404ConProblemDetails()
    {
        var response = await _client.GetAsync("/api/home/modules/invalid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(payload);
        Assert.Equal((int)HttpStatusCode.NotFound, payload!.Status);
        Assert.Equal("Module key not found", payload.Title);
        Assert.Contains("invalid", payload.Detail);
    }

    private sealed class ModuleResponse
    {
        public string RouteKey { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string RootElementTag { get; init; } = string.Empty;
    }
}
