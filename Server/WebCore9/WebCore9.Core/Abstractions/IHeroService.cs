using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHeroService
{
    IReadOnlyList<HeroDto> GetHeroes();

    IReadOnlyList<HeroDto> SearchHeroes(string? name, int? id);

    HeroDto? GetHeroById(int id);

    HeroMutationResult CreateHero(string? name);

    HeroMutationResult UpdateHero(int id, string? name);

    HeroMutationResult DeleteHero(int id);
}
