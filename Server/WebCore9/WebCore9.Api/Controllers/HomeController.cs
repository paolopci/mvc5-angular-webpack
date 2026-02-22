using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    public ActionResult<HomeInfoDto> Get()
    {
        var homeInfo = _homeService.GetHomeInfo();
        return Ok(homeInfo);
    }

    [HttpGet("modules/{key}")]
    [ProducesResponseType(typeof(ModuleInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ModuleInfoDto> GetModule(string key)
    {
        var moduleInfo = _homeService.GetModuleInfo(key);
        if (moduleInfo is null)
        {
            var problem = new ProblemDetails
            {
                Title = "Module key not found",
                Detail = $"Module key '{key}' is not supported.",
                Status = StatusCodes.Status404NotFound
            };

            return NotFound(problem);
        }

        return Ok(moduleInfo);
    }

    [HttpGet("loader")]
    [ProducesResponseType(typeof(LoaderInfoDto), StatusCodes.Status200OK)]
    public ActionResult<LoaderInfoDto> GetLoader()
    {
        var loaderInfo = _homeService.GetLoaderInfo();
        return Ok(loaderInfo);
    }

}
