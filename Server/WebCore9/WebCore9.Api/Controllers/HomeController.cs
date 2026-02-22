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

    [HttpGet("module1")]
    public ActionResult<ModuleInfoDto> GetModule1()
    {
        var moduleInfo = _homeService.GetModule1Info();
        return Ok(moduleInfo);
    }

    [HttpGet("module2")]
    public ActionResult<ModuleInfoDto> GetModule2()
    {
        var moduleInfo = _homeService.GetModule2Info();
        return Ok(moduleInfo);
    }
}
