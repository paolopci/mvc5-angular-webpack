using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests.TestSupport;

public static class ApiResponseTestHelper
{
    public static async Task<T> LeggiDataSuccessoAsync<T>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.Should().NotBeNull();

        return payload.Data!;
    }

    public static async Task<ProblemDetails> LeggiProblemDetailsAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        payload.Should().NotBeNull();
        return payload!;
    }
}
