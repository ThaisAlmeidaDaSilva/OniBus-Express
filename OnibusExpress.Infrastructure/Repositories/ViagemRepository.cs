using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Infrastructure.Data;

namespace OnibusExpress.Infrastructure.Repositories;

public sealed class ViagemRepository(OnibusDbContext dbContext) : IViagemRepository
{
    public async Task<IReadOnlyCollection<Viagem>> BuscarAsync(
        string origem,
        string destino,
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        var inicio = data.ToDateTime(TimeOnly.MinValue);
        var fim = data.ToDateTime(TimeOnly.MaxValue);

        return await dbContext.Viagens
            .AsNoTracking()
            .Include(viagem => viagem.Rota)
            .Include(viagem => viagem.Reservas)
            .Where(viagem =>
                viagem.Rota != null &&
                viagem.Rota.Origem == origem &&
                viagem.Rota.Destino == destino &&
                viagem.DataHoraPartida >= inicio &&
                viagem.DataHoraPartida <= fim)
            .OrderBy(viagem => viagem.DataHoraPartida)
            .ToListAsync(cancellationToken);
    }

    public async Task<Viagem?> ObterComReservasAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Viagens
            .Include(viagem => viagem.Rota)
            .Include(viagem => viagem.Reservas)
            .FirstOrDefaultAsync(viagem => viagem.Id == id, cancellationToken);
    }
}
