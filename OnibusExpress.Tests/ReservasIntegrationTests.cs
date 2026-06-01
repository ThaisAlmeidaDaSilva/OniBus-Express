using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Services;
using OnibusExpress.Controllers;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Infrastructure.Data;
using OnibusExpress.Infrastructure.Repositories;

namespace OnibusExpress.Tests;

public sealed partial class ReservasIntegrationTests
{
    [Fact]
    public async Task Criar_DeveBloquearAssentoJaOcupado()
    {
        await using var dbContext = CreateDbContext();
        var viagem = await SeedViagemAsync(dbContext, DateTime.Now.AddDays(1));
        var controller = CreateController(dbContext);

        var primeiraReserva = await controller.Criar(CreateRequest(viagem.Id, assentoNumero: 7));
        var segundaReserva = await controller.Criar(CreateRequest(viagem.Id, assentoNumero: 7));

        Assert.IsType<CreatedAtActionResult>(primeiraReserva.Result);
        var conflict = Assert.IsType<ConflictObjectResult>(segundaReserva.Result);
        Assert.Equal("Assento ja esta ocupado para esta viagem.", conflict.Value);
    }

    [Fact]
    public async Task Cancelar_DevePermitirCancelamentoAteDuasHorasAntesDaPartida()
    {
        await using var dbContext = CreateDbContext();
        var viagem = await SeedViagemAsync(dbContext, DateTime.Now.AddHours(3));
        var reserva = await SeedReservaAsync(dbContext, viagem, "ABC-12345");
        var controller = CreateController(dbContext);

        var result = await controller.Cancelar(reserva.Codigo);

        Assert.IsType<NoContentResult>(result);
        Assert.NotNull(reserva.CanceladaEm);
    }

    [Fact]
    public async Task Cancelar_DeveBloquearCancelamentoComMenosDeDuasHorasParaPartida()
    {
        await using var dbContext = CreateDbContext();
        var viagem = await SeedViagemAsync(dbContext, DateTime.Now.AddMinutes(90));
        var reserva = await SeedReservaAsync(dbContext, viagem, "ABC-12345");
        var controller = CreateController(dbContext);

        var result = await controller.Cancelar(reserva.Codigo);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cancelamento permitido somente ate 2 horas antes da partida.", badRequest.Value);
        Assert.Null(reserva.CanceladaEm);
    }

    [Fact]
    public async Task Criar_DeveGerarCodigoDeReservaUnicoELegivel()
    {
        await using var dbContext = CreateDbContext();
        var viagem = await SeedViagemAsync(dbContext, DateTime.Now.AddDays(1));
        var controller = CreateController(dbContext);

        var primeiraReserva = await controller.Criar(CreateRequest(viagem.Id, assentoNumero: 1));
        var segundaReserva = await controller.Criar(CreateRequest(viagem.Id, assentoNumero: 2));

        var primeira = GetCreatedReserva(primeiraReserva);
        var segunda = GetCreatedReserva(segundaReserva);

        Assert.Matches(ReservaCodeRegex(), primeira.Codigo);
        Assert.Matches(ReservaCodeRegex(), segunda.Codigo);
        Assert.NotEqual(primeira.Codigo, segunda.Codigo);
    }

    private static OnibusDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OnibusDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OnibusDbContext(options);
    }

    private static ReservasController CreateController(OnibusDbContext dbContext)
    {
        var service = new ReservasService(
            new ViagemRepository(dbContext),
            new ReservaRepository(dbContext),
            new TestClock());

        return new ReservasController(service);
    }

    private static async Task<Viagem> SeedViagemAsync(OnibusDbContext dbContext, DateTime dataHoraPartida)
    {
        var rota = new Rota
        {
            Origem = "Sao Paulo",
            Destino = "Rio de Janeiro",
            DuracaoEstimada = TimeSpan.FromHours(6)
        };

        var viagem = new Viagem
        {
            Rota = rota,
            DataHoraPartida = dataHoraPartida,
            Preco = 129.90m,
            TotalAssentos = 46
        };

        dbContext.Rotas.Add(rota);
        dbContext.Viagens.Add(viagem);
        await dbContext.SaveChangesAsync();

        return viagem;
    }

    private static async Task<Reserva> SeedReservaAsync(
        OnibusDbContext dbContext,
        Viagem viagem,
        string codigo)
    {
        var reserva = new Reserva
        {
            Codigo = codigo,
            Nome = "Maria Silva",
            Cpf = "529.982.247-25",
            Email = "maria@email.com",
            ViagemId = viagem.Id,
            Viagem = viagem,
            AssentoNumero = 1,
            CriadaEm = DateTime.UtcNow
        };

        dbContext.Reservas.Add(reserva);
        await dbContext.SaveChangesAsync();

        return reserva;
    }

    private static CriarReservaRequest CreateRequest(int viagemId, int assentoNumero)
    {
        return new CriarReservaRequest(
            Nome: "Maria Silva",
            Cpf: "529.982.247-25",
            Email: "maria@email.com",
            ViagemId: viagemId,
            AssentoNumero: assentoNumero);
    }

    private static ReservaDto GetCreatedReserva(ActionResult<ReservaDto> actionResult)
    {
        var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        return Assert.IsType<ReservaDto>(created.Value);
    }

    [GeneratedRegex("^[A-Z]{3}-\\d{5}$")]
    private static partial Regex ReservaCodeRegex();

    private sealed class TestClock : IClock
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
