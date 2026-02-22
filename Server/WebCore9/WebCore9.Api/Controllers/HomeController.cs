using Microsoft.AspNetCore.Mvc;
using WebCore9.Api.Common;
using WebCore9.Api.OpenApi;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HomeController : ApiControllerBase
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    [ProducesApiHomeInfoResponse]
    public ActionResult<ApiResponse<HomeInfoDto>> Get()
    {
        var homeInfo = _homeService.GetHomeInfo();
        return ApiOk(homeInfo);
    }

    [HttpGet("modules")]
    [ProducesApiModulesListResponse]
    public ActionResult<ApiResponse<IReadOnlyList<ModuleInfoDto>>> GetModules()
    {
        var modules = _homeService.GetModules();
        return ApiOk(modules);
    }

    [HttpGet("modules/{key}")]
    [ApiConventionMethod(typeof(HomeControllerOpenApiConventions), nameof(HomeControllerOpenApiConventions.GetModuleByKey))]
    public ActionResult<ApiResponse<ModuleInfoDto>> GetModule(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Module key is required."));
        }

        if (key.Contains('.') || key.Any(char.IsWhiteSpace))
        {
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed(
                "Module key must not contain whitespace or '.' characters."));
        }

        if (string.Equals(key.Trim(), "loader", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblem(ApiProblemDetailsFactory.Conflict(
                detail: "The key 'loader' is reserved for loader metadata. Use GET /api/home/loader.",
                title: "Reserved module key"));
        }

        var moduleInfo = _homeService.GetModuleInfo(key);
        if (moduleInfo is null)
        {
            return ApiNotFound(ApiProblemDetailsFactory.ModuleKeyNotFound(key));
        }

        return ApiOk(moduleInfo);
    }

    [HttpGet("loader")]
    [ApiConventionMethod(typeof(HomeControllerOpenApiConventions), nameof(HomeControllerOpenApiConventions.GetLoader))]
    public ActionResult<ApiResponse<LoaderInfoDto>> GetLoader()
    {
        try
        {
            var loaderInfo = _homeService.GetLoaderInfo();
            return ApiOk(loaderInfo);
        }
        catch (Exception)
        {
            return ApiProblem(ApiProblemDetailsFactory.InternalError("Unable to resolve loader metadata."));
        }
    }

    [HttpGet("loader/chunks")]
    [ApiConventionMethod(typeof(HomeControllerOpenApiConventions), nameof(HomeControllerOpenApiConventions.GetLoaderChunks))]
    public ActionResult<ApiResponse<LoaderChunkManifestDto>> GetLoaderChunks()
    {
        try
        {
            var chunkManifest = _homeService.GetLoaderChunkManifest();
            return ApiOk(chunkManifest);
        }
        catch (Exception)
        {
            return ApiProblem(ApiProblemDetailsFactory.InternalError("Unable to resolve loader chunk manifest."));
        }
    }

    [HttpGet("loader/html-plugin-config")]
    [ApiConventionMethod(typeof(HomeControllerOpenApiConventions), nameof(HomeControllerOpenApiConventions.GetLoaderHtmlPluginConfig))]
    public ActionResult<ApiResponse<LoaderHtmlPluginConfigDto>> GetLoaderHtmlPluginConfig()
    {
        try
        {
            var htmlPluginConfig = _homeService.GetLoaderHtmlPluginConfig();
            return ApiOk(htmlPluginConfig);
        }
        catch (Exception)
        {
            return ApiProblem(ApiProblemDetailsFactory.InternalError("Unable to resolve loader html plugin config."));
        }
    }

    [HttpGet("loader/config-diff")]
    [ApiConventionMethod(typeof(HomeControllerOpenApiConventions), nameof(HomeControllerOpenApiConventions.GetLoaderConfigDiff))]
    public ActionResult<ApiResponse<LoaderWebpackConfigDiffDto>> GetLoaderConfigDiff()
    {
        try
        {
            var configDiff = _homeService.GetLoaderWebpackConfigDiff();
            return ApiOk(configDiff);
        }
        catch (Exception)
        {
            return ApiProblem(ApiProblemDetailsFactory.InternalError("Unable to resolve loader webpack config diff."));
        }
    }

}
