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

    [HttpGet("modules/{key}")]
    [ProducesResponseType(typeof(ApiResponse<ModuleInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<ModuleInfoDto>> GetModule(string key)
    {
        var moduleInfo = _homeService.GetModuleInfo(key);
        if (moduleInfo is null)
        {
            return ApiNotFound(ApiProblemDetailsFactory.ModuleKeyNotFound(key));
        }

        return ApiOk(moduleInfo);
    }

    [HttpGet("loader")]
    [ProducesResponseType(typeof(ApiResponse<LoaderInfoDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<LoaderInfoDto>> GetLoader()
    {
        var loaderInfo = _homeService.GetLoaderInfo();
        return ApiOk(loaderInfo);
    }

}
