using Microsoft.AspNetCore.Mvc.Testing;

namespace WebCore9.Api.IntegrationTests.TestSupport;

public abstract class ApiIntegrationTestBase : IClassFixture<WebApplicationFactory<global::Program>>
{
    protected ApiIntegrationTestBase(WebApplicationFactory<global::Program> factory)
    {
        Client = CreateClient(factory);
    }

    protected HttpClient Client { get; }

    protected static HttpClient CreateClient(WebApplicationFactory<global::Program> factory)
    {
        return factory.CreateClient();
    }
}
