using Microsoft.AspNetCore.Mvc;

namespace WebCore9.Api.OpenApi;

public static class HomeControllerOpenApiConventions
{
    [ProducesApiModuleResponse]
    [ProducesApiBadRequest]
    [ProducesApiConflict]
    [ProducesApiNotFound]
    public static void GetModuleByKey(string key)
    {
    }

    [ProducesApiLoaderResponse]
    [ProducesApiInternalError]
    public static void GetLoader()
    {
    }

    [ProducesApiLoaderChunksResponse]
    [ProducesApiInternalError]
    public static void GetLoaderChunks()
    {
    }

    [ProducesApiLoaderHtmlPluginConfigResponse]
    [ProducesApiInternalError]
    public static void GetLoaderHtmlPluginConfig()
    {
    }

    [ProducesApiLoaderConfigDiffResponse]
    [ProducesApiInternalError]
    public static void GetLoaderConfigDiff()
    {
    }
}
