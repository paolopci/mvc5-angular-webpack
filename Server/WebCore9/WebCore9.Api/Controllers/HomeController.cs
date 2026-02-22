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
    public ActionResult<ModuleInfoDto> GetModule(string key)
    {
        var moduleInfo = _homeService.GetModuleInfo(key);
        if (moduleInfo is null)
        {
            return NotFound(new
            {
                message = "Module key not found.",
                key
            });
        }

        return Ok(moduleInfo);
    }

    // Compatibilita temporanea con i client che usano gli endpoint non parametrizzati.
    [HttpGet("module1")]
    public ActionResult<ModuleInfoDto> GetModule1()
    {
        return GetModule("module1");
    }

    [HttpGet("module2")]
    public ActionResult<ModuleInfoDto> GetModule2()
    {
        return GetModule("module2");
    }
}
