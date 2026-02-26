namespace WebCore9.Core.Models;

public enum HeroMutationErrorCode
{
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}

public sealed class HeroMutationResult
{
    public bool Success { get; init; }

    public HeroDto? Hero { get; init; }

    public HeroMutationErrorCode? ErrorCode { get; init; }

    public string? ErrorDetail { get; init; }

    public static HeroMutationResult Ok(HeroDto hero)
    {
        return new HeroMutationResult
        {
            Success = true,
            Hero = hero
        };
    }

    public static HeroMutationResult Fail(HeroMutationErrorCode errorCode, string detail)
    {
        return new HeroMutationResult
        {
            Success = false,
            ErrorCode = errorCode,
            ErrorDetail = detail
        };
    }
}
