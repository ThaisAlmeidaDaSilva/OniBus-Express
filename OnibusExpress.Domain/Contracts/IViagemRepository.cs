using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Contracts;

public interface IViagemRepository
{
    Task<IReadOnlyCollection<Viagem>> BuscarAsync(
        string origem,
        string destino,
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task<Viagem?> ObterComReservasAsync(int id, CancellationToken cancellationToken = default);
}
