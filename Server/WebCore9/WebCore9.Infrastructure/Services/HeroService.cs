using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HeroService : IHeroService
{
    private static readonly object SyncRoot = new();

    private readonly IHeroRepository _heroRepository;

    public HeroService(IHeroRepository heroRepository)
    {
        _heroRepository = heroRepository;
    }

    public IReadOnlyList<HeroDto> GetHeroes()
    {
        return _heroRepository.LoadHeroes();
    }

    public IReadOnlyList<HeroDto> SearchHeroes(string? name, int? id)
    {
        IEnumerable<HeroDto> query = _heroRepository.LoadHeroes();

        if (id.HasValue)
        {
            query = query.Where(h => h.Id == id.Value);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(h => h.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.ToArray();
    }

    public HeroDto? GetHeroById(int id)
    {
        var hero = _heroRepository.LoadHeroes().FirstOrDefault(h => h.Id == id);
        return hero is null ? null : Clone(hero);
    }

    public HeroMutationResult CreateHero(string? name)
    {
        var normalizedName = NormalizeName(name);
        if (normalizedName is null)
        {
            return HeroMutationResult.Fail(HeroMutationErrorCode.Validation, "Hero name is required.");
        }

        lock (SyncRoot)
        {
            var heroes = _heroRepository.LoadHeroes().Select(Clone).ToList();

            if (heroes.Any(h => string.Equals(h.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.Conflict, $"Hero name '{normalizedName}' already exists.");
            }

            var nextId = heroes.Count == 0 ? 11 : heroes.Max(h => h.Id) + 1;
            var created = new HeroDto { Id = nextId, Name = normalizedName };
            heroes.Add(created);

            _heroRepository.SaveHeroes(heroes);
            return HeroMutationResult.Ok(Clone(created));
        }
    }

    public HeroMutationResult UpdateHero(int id, string? name)
    {
        var normalizedName = NormalizeName(name);
        if (normalizedName is null)
        {
            return HeroMutationResult.Fail(HeroMutationErrorCode.Validation, "Hero name is required.");
        }

        lock (SyncRoot)
        {
            var heroes = _heroRepository.LoadHeroes().Select(Clone).ToList();
            var existing = heroes.FirstOrDefault(h => h.Id == id);

            if (existing is null)
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.NotFound, $"Hero id '{id}' is not supported.");
            }

            if (heroes.Any(h =>
                    h.Id != id &&
                    string.Equals(h.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.Conflict, $"Hero name '{normalizedName}' already exists.");
            }

            existing.Name = normalizedName;
            _heroRepository.SaveHeroes(heroes);

            return HeroMutationResult.Ok(Clone(existing));
        }
    }

    public HeroMutationResult DeleteHero(int id)
    {
        lock (SyncRoot)
        {
            var heroes = _heroRepository.LoadHeroes().Select(Clone).ToList();
            var existing = heroes.FirstOrDefault(h => h.Id == id);

            if (existing is null)
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.NotFound, $"Hero id '{id}' is not supported.");
            }

            heroes.Remove(existing);
            _heroRepository.SaveHeroes(heroes);

            return HeroMutationResult.Ok(Clone(existing));
        }
    }

    private static string? NormalizeName(string? name)
    {
        var normalized = name?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static HeroDto Clone(HeroDto hero)
    {
        return new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name
        };
    }
}
