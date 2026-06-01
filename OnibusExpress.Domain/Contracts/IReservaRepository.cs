using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Domain.Contracts;

public interface IReservaRepository
{
    Task<Reserva?> BuscarPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<bool> AssentoOcupadoAsync(int viagemId, int assentoNumero, CancellationToken cancellationToken = default);
    Task<bool> CodigoExisteAsync(string codigo, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
