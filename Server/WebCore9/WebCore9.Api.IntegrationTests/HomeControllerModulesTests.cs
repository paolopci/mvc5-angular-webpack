using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HomeControllerModulesTests : IClassFixture<WebApplicationFactory<global::Program>>
{
    private readonly HttpClient _client;

    public HomeControllerModulesTests(WebApplicationFactory<global::Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLoader_Restituisce200ConApiResponseELoaderInfo()
    {
        var response = await _client.GetAsync("/api/home/loader");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<LoaderInfoDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal("loader", payload.Data!.RouteKey);
        Assert.Equal("my-angular-app", payload.Data.RootElementTag);
        Assert.True(payload.Data.UsesWebpackChunkEntries);
    }

    [Fact]
    public async Task GetHealth_Restituisce200ConApiResponseEHealthStatus()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<HealthStatusDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal("Healthy", payload.Data!.Status);
        Assert.Equal("WebCore9.Api", payload.Data.Service);
    }

    [Fact]
    public async Task GetModule_ConChiaveValida_Restituisce200EPayloadAtteso()
    {
        var response = await _client.GetAsync("/api/home/modules/module1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<ModuleInfoDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal("module1", payload.Data!.RouteKey);
        Assert.Equal("Module 1", payload.Data.Title);
        Assert.Equal("my-angular-app", payload.Data.RootElementTag);
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

}
