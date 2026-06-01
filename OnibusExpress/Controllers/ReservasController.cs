using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Data;
using OnibusExpress.Dtos;
using OnibusExpress.Models;
using OnibusExpress.Services;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("reservas")]
public sealed class ReservasController(OnibusDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReservaDto>> Criar(CriarReservaRequest request)
    {
        var viagem = await dbContext.Viagens
            .Include(item => item.Rota)
            .FirstOrDefaultAsync(item => item.Id == request.ViagemId);

        if (viagem is null || viagem.Rota is null)
        {
            return NotFound("Viagem nao encontrada.");
        }

        if (viagem.DataHoraPartida <= DateTime.Now)
        {
            return BadRequest("Nao e possivel reservar passagem para uma viagem ja realizada.");
        }

        if (!CpfValidator.IsValid(request.Cpf))
        {
            return BadRequest("CPF invalido. Informe 11 digitos validos ou o formato 000.000.000-00.");
        }

        if (request.AssentoNumero > viagem.TotalAssentos)
        {
            return BadRequest($"O assento deve estar entre 1 e {viagem.TotalAssentos}.");
        }

        var assentoOcupado = await dbContext.Reservas.AnyAsync(reserva =>
            reserva.ViagemId == request.ViagemId &&
            reserva.AssentoNumero == request.AssentoNumero &&
            reserva.CanceladaEm == null);

        if (assentoOcupado)
        {
            return Conflict("Assento ja esta ocupado para esta viagem.");
        }

        var reserva = new Reserva
        {
            Codigo = await GerarCodigoUnicoAsync(),
            Nome = request.Nome.Trim(),
            Cpf = CpfValidator.Format(request.Cpf),
            Email = request.Email.Trim(),
            ViagemId = request.ViagemId,
            AssentoNumero = request.AssentoNumero,
            CriadaEm = DateTime.UtcNow
        };

        dbContext.Reservas.Add(reserva);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Nao foi possivel concluir a reserva. Verifique se o assento continua disponivel.");
        }

        reserva.Viagem = viagem;
        return CreatedAtAction(nameof(Consultar), new { codigo = reserva.Codigo }, Mapear(reserva));
    }

    [HttpGet("{codigo}")]
    public async Task<ActionResult<ReservaDto>> Consultar(string codigo)
    {
        var reserva = await BuscarPorCodigoAsync(codigo);

        if (reserva is null)
        {
            return NotFound();
        }

        return Ok(Mapear(reserva));
    }

    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Cancelar(string codigo)
    {
        var reserva = await BuscarPorCodigoAsync(codigo);

        if (reserva is null)
        {
            return NotFound();
        }

        if (reserva.CanceladaEm is not null)
        {
            return NoContent();
        }

        if (reserva.Viagem is null || DateTime.Now > reserva.Viagem.DataHoraPartida.AddHours(-2))
        {
            return BadRequest("Cancelamento permitido somente ate 2 horas antes da partida.");
        }

        reserva.CanceladaEm = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Reserva?> BuscarPorCodigoAsync(string codigo)
    {
        return await dbContext.Reservas
            .Include(reserva => reserva.Viagem)
            .ThenInclude(viagem => viagem!.Rota)
            .FirstOrDefaultAsync(reserva => reserva.Codigo == codigo);
    }

    private async Task<string> GerarCodigoUnicoAsync()
    {
        string codigo;

        do
        {
            codigo = $"{GerarLetras(3)}-{Random.Shared.Next(10000, 100000)}";
        }
        while (await dbContext.Reservas.AnyAsync(reserva => reserva.Codigo == codigo));

        return codigo;
    }

    private static string GerarLetras(int quantidade)
    {
        const string letras = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        return new string(Enumerable
            .Range(0, quantidade)
            .Select(_ => letras[Random.Shared.Next(letras.Length)])
            .ToArray());
    }

    private static ReservaDto Mapear(Reserva reserva)
    {
        var viagem = reserva.Viagem;
        var rota = viagem?.Rota;

        return new ReservaDto(
            reserva.Codigo,
            reserva.Nome,
            reserva.Cpf,
            reserva.Email,
            reserva.ViagemId,
            rota?.Origem ?? string.Empty,
            rota?.Destino ?? string.Empty,
            viagem?.DataHoraPartida ?? default,
            reserva.AssentoNumero,
            reserva.CriadaEm,
            reserva.CanceladaEm,
            reserva.EstaAtiva ? "Ativa" : "Cancelada");
    }
}
