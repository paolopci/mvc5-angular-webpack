using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HeroService : IHeroService
{
    private static readonly object SyncRoot = new();

    private static readonly HeroDto[] SeedHeroes =
    [
        new() { Id = 11, Name = "Mr. Nice" },
        new() { Id = 12, Name = "Narco" },
        new() { Id = 13, Name = "Bombasto" },
        new() { Id = 14, Name = "Celeritas" },
        new() { Id = 15, Name = "Magneta" },
        new() { Id = 16, Name = "RubberMan" },
        new() { Id = 17, Name = "Dynama" },
        new() { Id = 18, Name = "Dr IQ" },
        new() { Id = 19, Name = "Magma" },
        new() { Id = 20, Name = "Tornado" }
    ];

    private static List<HeroDto> _heroes = SeedHeroes.Select(Clone).ToList();

    public IReadOnlyList<HeroDto> GetHeroes()
    {
        lock (SyncRoot)
        {
            return _heroes.Select(Clone).ToArray();
        }
    }

    public IReadOnlyList<HeroDto> SearchHeroes(string? name, int? id)
    {
        HeroDto[] snapshot;
        lock (SyncRoot)
        {
            snapshot = _heroes.Select(Clone).ToArray();
        }

        IEnumerable<HeroDto> query = snapshot;

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
        lock (SyncRoot)
        {
            var hero = _heroes.FirstOrDefault(h => h.Id == id);
            return hero is null ? null : Clone(hero);
        }
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
            if (_heroes.Any(h => string.Equals(h.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.Conflict, $"Hero name '{normalizedName}' already exists.");
            }

            var nextId = _heroes.Count == 0 ? 11 : _heroes.Max(h => h.Id) + 1;
            var created = new HeroDto { Id = nextId, Name = normalizedName };
            _heroes.Add(created);

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
            var existing = _heroes.FirstOrDefault(h => h.Id == id);
            if (existing is null)
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.NotFound, $"Hero id '{id}' is not supported.");
            }

            if (_heroes.Any(h =>
                    h.Id != id &&
                    string.Equals(h.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.Conflict, $"Hero name '{normalizedName}' already exists.");
            }

            existing.Name = normalizedName;
            return HeroMutationResult.Ok(Clone(existing));
        }
    }

    public HeroMutationResult DeleteHero(int id)
    {
        lock (SyncRoot)
        {
            var existing = _heroes.FirstOrDefault(h => h.Id == id);
            if (existing is null)
            {
                return HeroMutationResult.Fail(HeroMutationErrorCode.NotFound, $"Hero id '{id}' is not supported.");
            }

            _heroes.Remove(existing);
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
