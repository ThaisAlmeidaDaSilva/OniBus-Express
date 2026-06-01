using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnibusExpress.Data;
using OnibusExpress.Dtos;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("rotas")]
public sealed class RotasController(OnibusDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RotaDto>>> Listar()
    {
        var rotas = await dbContext.Rotas
            .AsNoTracking()
            .OrderBy(rota => rota.Origem)
            .ThenBy(rota => rota.Destino)
            .Select(rota => new RotaDto(
                rota.Id,
                rota.Origem,
                rota.Destino,
                rota.DuracaoEstimada.ToString(@"hh\:mm")))
            .ToListAsync();

        return Ok(rotas);
    }
}
