using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HeroService : IHeroService
{
    private static readonly HeroDto[] Heroes =
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

    public IReadOnlyList<HeroDto> GetHeroes()
    {
        return Heroes;
    }

    public IReadOnlyList<HeroDto> SearchHeroes(string? name, int? id)
    {
        IEnumerable<HeroDto> query = Heroes;

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
        return Heroes.FirstOrDefault(h => h.Id == id);
    }
}
