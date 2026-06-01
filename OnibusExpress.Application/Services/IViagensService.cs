using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;

namespace OnibusExpress.Application.Services;

public interface IViagensService
{
    Task<ApplicationResult<IReadOnlyCollection<ViagemResumoDto>>> BuscarAsync(
        string origem,
        string destino,
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task<ApplicationResult<ViagemDetalheDto>> DetalharAsync(int id, CancellationToken cancellationToken = default);
}
