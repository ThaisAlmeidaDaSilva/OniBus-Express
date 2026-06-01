using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;
using OnibusExpress.Application.Services;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("viagens")]
public sealed class ViagensController(IViagensService viagensService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ViagemResumoDto>>> Buscar(
        [FromQuery] string origem,
        [FromQuery] string destino,
        [FromQuery] DateOnly data,
        CancellationToken cancellationToken = default)
    {
        var result = await viagensService.BuscarAsync(origem, destino, data, cancellationToken);

        return result.Status == ApplicationStatus.BadRequest
            ? BadRequest(result.Error)
            : Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ViagemDetalheDto>> Detalhar(int id, CancellationToken cancellationToken = default)
    {
        var result = await viagensService.DetalharAsync(id, cancellationToken);

        return result.Status == ApplicationStatus.NotFound
            ? NotFound()
            : Ok(result.Value);
    }
}
