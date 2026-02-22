using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHeroRepository
{
    IReadOnlyList<HeroDto> LoadHeroes();

    void SaveHeroes(IReadOnlyList<HeroDto> heroes);
}
