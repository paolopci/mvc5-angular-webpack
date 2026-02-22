using Microsoft.AspNetCore.Mvc;
using WebCore9.Core.Models;

namespace WebCore9.Api.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ProducesApiOkResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesApiOkResponseAttribute(Type dataType)
        : base(typeof(ApiResponse<>).MakeGenericType(dataType), StatusCodes.Status200OK)
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiHealthResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiHealthResponseAttribute()
        : base(typeof(HealthStatusDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiHomeInfoResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiHomeInfoResponseAttribute()
        : base(typeof(HomeInfoDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiModulesListResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiModulesListResponseAttribute()
        : base(typeof(IReadOnlyList<ModuleInfoDto>))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiModuleResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiModuleResponseAttribute()
        : base(typeof(ModuleInfoDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiHeroesListResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiHeroesListResponseAttribute()
        : base(typeof(IReadOnlyList<HeroDto>))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiHeroResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiHeroResponseAttribute()
        : base(typeof(HeroDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiLoaderResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiLoaderResponseAttribute()
        : base(typeof(LoaderInfoDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiLoaderChunksResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiLoaderChunksResponseAttribute()
        : base(typeof(LoaderChunkManifestDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiLoaderHtmlPluginConfigResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiLoaderHtmlPluginConfigResponseAttribute()
        : base(typeof(LoaderHtmlPluginConfigDto))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesApiLoaderConfigDiffResponseAttribute : ProducesApiOkResponseAttribute
{
    public ProducesApiLoaderConfigDiffResponseAttribute()
        : base(typeof(LoaderWebpackConfigDiffDto))
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
