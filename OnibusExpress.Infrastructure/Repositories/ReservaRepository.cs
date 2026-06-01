using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Infrastructure.Data;

namespace OnibusExpress.Infrastructure.Repositories;

public sealed class ReservaRepository(OnibusDbContext dbContext) : IReservaRepository
{
    public async Task<Reserva?> BuscarPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservas
            .Include(reserva => reserva.Viagem)
            .ThenInclude(viagem => viagem!.Rota)
            .FirstOrDefaultAsync(reserva => reserva.Codigo == codigo, cancellationToken);
    }

    public async Task<bool> AssentoOcupadoAsync(
        int viagemId,
        int assentoNumero,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservas.AnyAsync(reserva =>
            reserva.ViagemId == viagemId &&
            reserva.AssentoNumero == assentoNumero &&
            reserva.CanceladaEm == null,
            cancellationToken);
    }

    public async Task<bool> CodigoExisteAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservas.AnyAsync(reserva => reserva.Codigo == codigo, cancellationToken);
    }

    public async Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        await dbContext.Reservas.AddAsync(reserva, cancellationToken);
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new ReservaConflitoException("Falha ao salvar reserva.", exception);
        }
    }
}
