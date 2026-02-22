using Microsoft.AspNetCore.Mvc;
using WebCore9.Api.Common;
using WebCore9.Api.OpenApi;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HeroesController : ApiControllerBase
{
    private readonly IHeroService _heroService;

    public HeroesController(IHeroService heroService)
    {
        _heroService = heroService;
    }

    [HttpGet]
    [ProducesApiHeroesListResponse]
    public ActionResult<ApiResponse<IReadOnlyList<HeroDto>>> GetHeroes(
        [FromQuery] string? name = null,
        [FromQuery] int? id = null)
    {
        var heroes = (id.HasValue || !string.IsNullOrWhiteSpace(name))
            ? _heroService.SearchHeroes(name, id)
            : _heroService.GetHeroes();

        return ApiOk(heroes);
    }

    [HttpGet("{id:int}")]
    [ProducesApiHeroResponse]
    [ProducesApiBadRequest]
    [ProducesApiNotFound]
    public ActionResult<ApiResponse<HeroDto>> GetHeroById(int id)
    {
        if (id <= 0)
        {
            return ApiProblem(ApiProblemDetailsFactory.ValidationFailed("Hero id must be greater than 0."));
        }

        var hero = _heroService.GetHeroById(id);
        if (hero is null)
        {
            return ApiNotFound(ApiProblemDetailsFactory.HeroIdNotFound(id));
        }

        return ApiOk(hero);
    }
}
