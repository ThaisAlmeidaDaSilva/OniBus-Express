using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Data;
using OnibusExpress.Dtos;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("viagens")]
public sealed class ViagensController(OnibusDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ViagemResumoDto>>> Buscar(
        [FromQuery] string origem,
        [FromQuery] string destino,
        [FromQuery] DateOnly data)
    {
        if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(destino))
        {
            return BadRequest("Informe origem, destino e data.");
        }

        var inicio = data.ToDateTime(TimeOnly.MinValue);
        var fim = data.ToDateTime(TimeOnly.MaxValue);

        var viagens = await dbContext.Viagens
            .AsNoTracking()
            .Include(viagem => viagem.Rota)
            .Include(viagem => viagem.Reservas)
            .Where(viagem =>
                viagem.Rota != null &&
                viagem.Rota.Origem == origem &&
                viagem.Rota.Destino == destino &&
                viagem.DataHoraPartida >= inicio &&
                viagem.DataHoraPartida <= fim)
            .OrderBy(viagem => viagem.DataHoraPartida)
            .Select(viagem => new ViagemResumoDto(
                viagem.Id,
                viagem.RotaId,
                viagem.Rota!.Origem,
                viagem.Rota.Destino,
                viagem.DataHoraPartida,
                viagem.Preco,
                viagem.TotalAssentos,
                viagem.TotalAssentos - viagem.Reservas.Count(reserva => reserva.CanceladaEm == null)))
            .ToListAsync();

        return Ok(viagens);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ViagemDetalheDto>> Detalhar(int id)
    {
        var viagem = await dbContext.Viagens
            .AsNoTracking()
            .Include(item => item.Rota)
            .Include(item => item.Reservas)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (viagem is null || viagem.Rota is null)
        {
            return NotFound();
        }

        var ocupados = viagem.Reservas
            .Where(reserva => reserva.CanceladaEm is null)
            .Select(reserva => reserva.AssentoNumero)
            .Order()
            .ToArray();

        var livres = Enumerable.Range(1, viagem.TotalAssentos)
            .Except(ocupados)
            .ToArray();

        return Ok(new ViagemDetalheDto(
            viagem.Id,
            viagem.Rota.Origem,
            viagem.Rota.Destino,
            viagem.DataHoraPartida,
            viagem.Preco,
            viagem.TotalAssentos,
            livres,
            ocupados));
    }
}
