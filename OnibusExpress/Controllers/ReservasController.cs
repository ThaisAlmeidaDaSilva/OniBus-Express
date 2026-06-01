using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;
using OnibusExpress.Application.Services;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("reservas")]
public sealed class ReservasController(IReservasService reservasService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReservaDto>> Criar(
        CriarReservaRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await reservasService.CriarAsync(request, cancellationToken);

        return result.Status switch
        {
            ApplicationStatus.Created => CreatedAtAction(
                nameof(Consultar),
                new { codigo = result.Value!.Codigo },
                result.Value),
            ApplicationStatus.NotFound => NotFound(result.Error),
            ApplicationStatus.BadRequest => BadRequest(result.Error),
            ApplicationStatus.Conflict => Conflict(result.Error),
            _ => Ok(result.Value)
        };
    }

    [HttpGet("{codigo}")]
    public async Task<ActionResult<ReservaDto>> Consultar(string codigo, CancellationToken cancellationToken = default)
    {
        var result = await reservasService.ConsultarAsync(codigo, cancellationToken);

        return result.Status == ApplicationStatus.NotFound
            ? NotFound()
            : Ok(result.Value);
    }

    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Cancelar(string codigo, CancellationToken cancellationToken = default)
    {
        var result = await reservasService.CancelarAsync(codigo, cancellationToken);

        return result.Status switch
        {
            ApplicationStatus.NotFound => NotFound(),
            ApplicationStatus.BadRequest => BadRequest(result.Error),
            _ => NoContent()
        };
    }
}
