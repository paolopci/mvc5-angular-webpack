using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebCore9.Api.IntegrationTests.TestSupport;
using WebCore9.Core.Models;

namespace WebCore9.Api.IntegrationTests;

public sealed class HeroesControllerTests : ApiIntegrationTestBase
{
    public HeroesControllerTests(WebApplicationFactory<global::Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetHeroes_QuandoRichiesto_AlloraRestituisce200ConListaHeroWrappata()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/heroes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<IReadOnlyList<HeroDto>>(response);
        data.Should().HaveCountGreaterOrEqualTo(10);
        data.Should().Contain(h => h.Id == 11 && h.Name == "Mr. Nice");
        data.Should().Contain(h => h.Id == 20 && h.Name == "Tornado");
    }

    [Fact]
    public async Task GetHeroes_QuandoFiltroId_AlloraRestituisceArrayConSingoloElemento()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/heroes?id=11");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<IReadOnlyList<HeroDto>>(response);
        data.Should().ContainSingle();
        data[0].Id.Should().Be(11);
        data[0].Name.Should().Be("Mr. Nice");
    }

    [Fact]
    public async Task GetHeroes_QuandoFiltroNome_AlloraRestituisceElementiCompatibiliConRicerca()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/heroes?name=tor");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<IReadOnlyList<HeroDto>>(response);
        data.Should().ContainSingle();
        data[0].Name.Should().Be("Tornado");
    }

    [Fact]
    public async Task GetHeroById_QuandoIdValido_AlloraRestituisce200ConHero()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/heroes/12");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(response);
        data.Id.Should().Be(12);
        data.Name.Should().Be("Narco");
    }

    [Fact]
    public async Task GetHeroById_QuandoIdAssente_AlloraRestituisce404ConProblemDetails()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/api/heroes/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var payload = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        payload.Status.Should().Be((int)HttpStatusCode.NotFound);
        payload.Title.Should().Be("Hero not found");
        payload.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task PostHero_QuandoPayloadValido_AlloraCreaHeroErestituisce201()
    {
        // Arrange
        var payload = new { name = $"Nuovo Hero {Guid.NewGuid():N}" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/heroes", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(response);
        created.Id.Should().BeGreaterThan(20);
        created.Name.Should().Be(payload.name);

        var fetchResponse = await Client.GetAsync($"/api/heroes/{created.Id}");
        fetchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostHero_QuandoNomeVuoto_AlloraRestituisce400()
    {
        // Arrange
        var payload = new { name = "   " };

        // Act
        var response = await Client.PostAsJsonAsync("/api/heroes", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problem.Title.Should().Be("Validation failed");
    }

    [Fact]
    public async Task PostHero_QuandoNomeDuplicato_AlloraRestituisce409()
    {
        // Arrange
        var payload = new { name = "Mr. Nice" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/heroes", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
        problem.Title.Should().Be("Hero conflict");
    }

    [Fact]
    public async Task PutHero_QuandoPayloadValido_AlloraAggiornaHeroErestituisce200()
    {
        // Arrange
        var id = 12;
        var updatedName = $"Narco Updated {Guid.NewGuid():N}"[..20];
        var payload = new { id, name = updatedName };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/heroes/{id}", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(response);
        updated.Id.Should().Be(id);
        updated.Name.Should().Be(updatedName);

        var fetchResponse = await Client.GetAsync($"/api/heroes/{id}");
        var fetched = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(fetchResponse);
        fetched.Name.Should().Be(updatedName);
    }

    [Fact]
    public async Task PutHero_QuandoBodyIdMismatch_AlloraRestituisce409()
    {
        // Arrange
        var payload = new { id = 999, name = "Mismatch" };

        // Act
        var response = await Client.PutAsJsonAsync("/api/heroes/12", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutHero_QuandoIdInesistente_AlloraRestituisce404()
    {
        // Arrange
        var payload = new { id = 999, name = "Ghost Hero" };

        // Act
        var response = await Client.PutAsJsonAsync("/api/heroes/999", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("Hero not found");
    }

    [Fact]
    public async Task PutHero_QuandoNomeDuplicato_AlloraRestituisce409()
    {
        // Arrange
        var payload = new { id = 12, name = "Tornado" };

        // Act
        var response = await Client.PutAsJsonAsync("/api/heroes/12", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteHero_QuandoIdValido_AlloraRimuoveHeroErestituisce200()
    {
        // Arrange
        var createResponse = await Client.PostAsJsonAsync("/api/heroes", new { name = $"Delete Me {Guid.NewGuid():N}" });
        var created = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(createResponse);

        // Act
        var deleteResponse = await Client.DeleteAsync($"/api/heroes/{created.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var deleted = await ApiResponseTestHelper.LeggiDataSuccessoAsync<HeroDto>(deleteResponse);
        deleted.Id.Should().Be(created.Id);

        var fetchResponse = await Client.GetAsync($"/api/heroes/{created.Id}");
        fetchResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHero_QuandoIdInesistente_AlloraRestituisce404()
    {
        // Arrange

        // Act
        var response = await Client.DeleteAsync("/api/heroes/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await ApiResponseTestHelper.LeggiProblemDetailsAsync(response);
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
    }
}
