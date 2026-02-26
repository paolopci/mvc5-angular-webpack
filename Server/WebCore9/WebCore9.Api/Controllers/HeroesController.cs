using Microsoft.AspNetCore.Mvc;
using WebCore9.Api.Common;
using WebCore9.Api.OpenApi;
using WebCore9.Api.Telemetry;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HeroesController : ApiControllerBase
{
    private readonly IHeroService _heroService;
    private readonly ILogger<HeroesController> _logger;

    public HeroesController(IHeroService heroService, ILogger<HeroesController> logger)
    {
        _heroService = heroService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesApiHeroesListResponse]
    public ActionResult<ApiResponse<IReadOnlyList<HeroDto>>> GetHeroes(
        [FromQuery] string? name = null,
        [FromQuery] int? id = null)
    {
        const string operation = "list";

        var heroes = (id.HasValue || !string.IsNullOrWhiteSpace(name))
            ? _heroService.SearchHeroes(name, id)
            : _heroService.GetHeroes();

        HeroMetrics.TrackRequest(operation, "success");
        return ApiOk(heroes);
    }

    [HttpGet("{id:int}")]
    [ProducesApiHeroResponse]
    [ProducesApiBadRequest]
    [ProducesApiNotFound]
    public ActionResult<ApiResponse<HeroDto>> GetHeroById(int id)
    {
        const string operation = "getById";

        if (id <= 0)
        {
            HeroMetrics.TrackError(operation, "validation");
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Hero id must be greater than 0."));
        }

        var hero = _heroService.GetHeroById(id);
        if (hero is null)
        {
            HeroMetrics.TrackError(operation, "not_found");
            return ApiNotFound(ApiProblemDetailsFactory.HeroIdNotFound(id));
        }

        HeroMetrics.TrackRequest(operation, "success");
        return ApiOk(hero);
    }

    [HttpPost]
    [ProducesApiCreatedHeroResponse]
    [ProducesApiBadRequest]
    [ProducesApiConflict]
    public ActionResult<ApiResponse<HeroDto>> CreateHero([FromBody] HeroMutationRequestDto request)
    {
        const string operation = "create";

        if (request.Id.HasValue)
        {
            HeroMetrics.TrackError(operation, "validation");
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Id must not be provided when creating a hero."));
        }

        var result = _heroService.CreateHero(request.Name);
        if (!result.Success)
        {
            return MapMutationFailure(operation, 0, result);
        }

        var hero = result.Hero!;
        _logger.LogInformation("Heroes create succeeded for {HeroId}", hero.Id);
        HeroMetrics.TrackMutation(operation, "success");
        return ApiCreated($"/api/heroes/{hero.Id}", hero);
    }

    [HttpPut("{id:int}")]
    [ProducesApiHeroResponse]
    [ProducesApiBadRequest]
    [ProducesApiNotFound]
    [ProducesApiConflict]
    public ActionResult<ApiResponse<HeroDto>> UpdateHero(int id, [FromBody] HeroMutationRequestDto request)
    {
        const string operation = "update";

        if (id <= 0)
        {
            HeroMetrics.TrackError(operation, "validation");
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Hero id must be greater than 0."));
        }

        if (request.Id.HasValue && request.Id.Value != id)
        {
            HeroMetrics.TrackError(operation, "conflict");
            return ApiProblem(ApiProblemDetailsFactory.Conflict("Route id and body id must match."));
        }

        var result = _heroService.UpdateHero(id, request.Name);
        if (!result.Success)
        {
            return MapMutationFailure(operation, id, result);
        }

        var hero = result.Hero!;
        _logger.LogInformation("Heroes update succeeded for {HeroId}", hero.Id);
        HeroMetrics.TrackMutation(operation, "success");
        return ApiOk(hero);
    }

    [HttpDelete("{id:int}")]
    [ProducesApiHeroResponse]
    [ProducesApiBadRequest]
    [ProducesApiNotFound]
    public ActionResult<ApiResponse<HeroDto>> DeleteHero(int id)
    {
        const string operation = "delete";

        if (id <= 0)
        {
            HeroMetrics.TrackError(operation, "validation");
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Hero id must be greater than 0."));
        }

        var result = _heroService.DeleteHero(id);
        if (!result.Success)
        {
            return MapMutationFailure(operation, id, result);
        }

        var hero = result.Hero!;
        _logger.LogInformation("Heroes delete succeeded for {HeroId}", hero.Id);
        HeroMetrics.TrackMutation(operation, "success");
        return ApiOk(hero);
    }

    private ActionResult<ApiResponse<HeroDto>> MapMutationFailure(string operation, int id, HeroMutationResult result)
    {
        var detail = result.ErrorDetail ?? "Hero mutation failed.";

        switch (result.ErrorCode)
        {
            case HeroMutationErrorCode.Validation:
                _logger.LogWarning("Heroes {Operation} validation failed. Id={HeroId}. Detail={Detail}", operation, id, detail);
                HeroMetrics.TrackError(operation, "validation");
                return ApiProblem(ApiProblemDetailsFactory.ValidationFailed(detail));

            case HeroMutationErrorCode.NotFound:
                _logger.LogWarning("Heroes {Operation} target not found. Id={HeroId}. Detail={Detail}", operation, id, detail);
                HeroMetrics.TrackError(operation, "not_found");
                return ApiNotFound(ApiProblemDetailsFactory.HeroIdNotFound(id));

            case HeroMutationErrorCode.Conflict:
                _logger.LogWarning("Heroes {Operation} conflict. Id={HeroId}. Detail={Detail}", operation, id, detail);
                HeroMetrics.TrackError(operation, "conflict");
                return ApiProblem(ApiProblemDetailsFactory.Conflict(detail, "Hero conflict"));

            default:
                _logger.LogError("Heroes {Operation} failed with unexpected error state. Id={HeroId}. Detail={Detail}", operation, id, detail);
                HeroMetrics.TrackError(operation, "unknown");
                return ApiProblem(ApiProblemDetailsFactory.InternalError(detail));
        }
    }
}
