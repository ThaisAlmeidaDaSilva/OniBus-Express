using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;

namespace OnibusExpress.Application.Services;

public interface IReservasService
{
    Task<ApplicationResult<ReservaDto>> CriarAsync(
        CriarReservaRequest request,
        CancellationToken cancellationToken = default);

    Task<ApplicationResult<ReservaDto>> ConsultarAsync(string codigo, CancellationToken cancellationToken = default);
    Task<ApplicationResult> CancelarAsync(string codigo, CancellationToken cancellationToken = default);
}
