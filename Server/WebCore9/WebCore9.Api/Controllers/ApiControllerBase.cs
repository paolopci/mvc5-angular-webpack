using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> ApiOk<T>(T data)
    {
        return Ok(ApiResponse<T>.Ok(data));
    }

    protected ObjectResult ApiProblem(ProblemDetails problemDetails)
    {
        var statusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        return StatusCode(statusCode, problemDetails);
    }

    protected ActionResult ApiNotFound(ProblemDetails problemDetails)
    {
        return NotFound(problemDetails);
    }
}
