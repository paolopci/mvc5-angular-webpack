using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Models;

namespace WebCore9.Api.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class ProducesApiOkResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesApiOkResponseAttribute(Type dataType)
        : base(typeof(ApiResponse<>).MakeGenericType(dataType), StatusCodes.Status200OK)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ProducesApiProblemResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesApiProblemResponseAttribute(int statusCode)
        : base(typeof(ProblemDetails), statusCode)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiBadRequestAttribute : ProducesApiProblemResponseAttribute
{
    public ProducesApiBadRequestAttribute()
        : base(StatusCodes.Status400BadRequest)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiNotFoundAttribute : ProducesApiProblemResponseAttribute
{
    public ProducesApiNotFoundAttribute()
        : base(StatusCodes.Status404NotFound)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiConflictAttribute : ProducesApiProblemResponseAttribute
{
    public ProducesApiConflictAttribute()
        : base(StatusCodes.Status409Conflict)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiInternalErrorAttribute : ProducesApiProblemResponseAttribute
{
    public ProducesApiInternalErrorAttribute()
        : base(StatusCodes.Status500InternalServerError)
    {
    }
}
