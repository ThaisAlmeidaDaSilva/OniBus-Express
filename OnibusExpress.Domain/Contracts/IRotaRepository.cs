using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Contracts;

public interface IRotaRepository
{
    Task<IReadOnlyCollection<Rota>> ListarAsync(CancellationToken cancellationToken = default);
}
