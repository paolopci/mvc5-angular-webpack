using System.Net;
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
        data.Should().HaveCount(10);
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
}
