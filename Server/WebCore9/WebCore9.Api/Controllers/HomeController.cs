using Microsoft.AspNetCore.Mvc;
using WebCore9.Api.Common;
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
    [ProducesResponseType(typeof(ApiResponse<HomeInfoDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HomeInfoDto>> Get()
    {
        var homeInfo = _homeService.GetHomeInfo();
        return ApiOk(homeInfo);
    }

    [HttpGet("modules")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ModuleInfoDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<ModuleInfoDto>>> GetModules()
    {
        var modules = _homeService.GetModules();
        return ApiOk(modules);
    }

    [HttpGet("modules/{key}")]
    [ProducesResponseType(typeof(ApiResponse<ModuleInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(ApiResponse<LoaderInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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

}
