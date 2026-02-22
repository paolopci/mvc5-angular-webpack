using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;
    private readonly IHostEnvironment _environment;

    public HealthController(IHealthService healthService, IHostEnvironment environment)
    {
        _healthService = healthService;
        _environment = environment;
    }

    [HttpGet]
    public ActionResult<HealthStatusDto> Get()
    {
        var status = _healthService.GetStatus(_environment.EnvironmentName);
        return Ok(status);
    }
}
