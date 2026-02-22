using Microsoft.AspNetCore.Mvc;
using WebCore9.Api.OpenApi;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ApiControllerBase
{
    private readonly IHealthService _healthService;
    private readonly IHostEnvironment _environment;

    public HealthController(IHealthService healthService, IHostEnvironment environment)
    {
        _healthService = healthService;
        _environment = environment;
    }

    [HttpGet]
    [ProducesApiOkResponse(typeof(HealthStatusDto))]
    public ActionResult<ApiResponse<HealthStatusDto>> Get()
    {
        var status = _healthService.GetStatus(_environment.EnvironmentName);
        return ApiOk(status);
    }
}
