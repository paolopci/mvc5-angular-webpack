using Microsoft.AspNetCore.Mvc;

namespace WebCore9.Api.Common;

public static class ApiProblemDetailsFactory
{
    public static ProblemDetails ModuleKeyNotFound(string key)
    {
        return new ProblemDetails
        {
            Title = "Module key not found",
            Detail = $"Module key '{key}' is not supported.",
            Status = StatusCodes.Status404NotFound
        };
    }

    public static ProblemDetails HeroIdNotFound(int id)
    {
        return new ProblemDetails
        {
            Title = "Hero not found",
            Detail = $"Hero id '{id}' is not supported.",
            Status = StatusCodes.Status404NotFound
        };
    }

    public static ProblemDetails ValidationFailed(string detail, string? title = null)
    {
        return new ProblemDetails
        {
            Title = title ?? "Validation failed",
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };
    }

    public static ProblemDetails Conflict(string detail, string? title = null)
    {
        return new ProblemDetails
        {
            Title = title ?? "Conflict",
            Detail = detail,
            Status = StatusCodes.Status409Conflict
        };
    }

    public static ProblemDetails InternalError(string detail, string? title = null)
    {
        return new ProblemDetails
        {
            Title = title ?? "Internal server error",
            Detail = detail,
            Status = StatusCodes.Status500InternalServerError
        };
    }
}
