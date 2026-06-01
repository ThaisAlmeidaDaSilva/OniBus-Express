using Microsoft.AspNetCore.Mvc;
using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Services;

namespace OnibusExpress.Controllers;

[ApiController]
[Route("rotas")]
public sealed class RotasController(IRotasService rotasService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RotaDto>>> Listar(CancellationToken cancellationToken = default)
    {
        var rotas = await rotasService.ListarAsync(cancellationToken);
        return Ok(rotas);
    }
}
