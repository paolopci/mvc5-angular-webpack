using System.Text.Json;
using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Repositories;

public sealed class JsonFileHeroRepository : IHeroRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _storageFilePath;

    public JsonFileHeroRepository(string storageFilePath)
    {
        _storageFilePath = storageFilePath;
        EnsureStoreExists();
    }

    public IReadOnlyList<HeroDto> LoadHeroes()
    {
        EnsureStoreExists();

        var json = File.ReadAllText(_storageFilePath);
        var heroes = JsonSerializer.Deserialize<List<HeroDto>>(json, JsonOptions) ?? [];

        return heroes
            .Select(Clone)
            .OrderBy(h => h.Id)
            .ToArray();
    }

    public void SaveHeroes(IReadOnlyList<HeroDto> heroes)
    {
        var directory = Path.GetDirectoryName(_storageFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var normalized = heroes
            .Select(Clone)
            .OrderBy(h => h.Id)
            .ToArray();

        var json = JsonSerializer.Serialize(normalized, JsonOptions);
        File.WriteAllText(_storageFilePath, json);
    }

    private void EnsureStoreExists()
    {
        var directory = Path.GetDirectoryName(_storageFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(_storageFilePath))
        {
            return;
        }

        var seed = GetSeedHeroes();
        var json = JsonSerializer.Serialize(seed, JsonOptions);
        File.WriteAllText(_storageFilePath, json);
    }

    private static HeroDto[] GetSeedHeroes()
    {
        return
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
