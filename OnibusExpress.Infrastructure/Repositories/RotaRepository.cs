using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Infrastructure.Data;

namespace OnibusExpress.Infrastructure.Repositories;

public sealed class RotaRepository(OnibusDbContext dbContext) : IRotaRepository
{
    public async Task<IReadOnlyCollection<Rota>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Rotas
            .AsNoTracking()
            .OrderBy(rota => rota.Origem)
            .ThenBy(rota => rota.Destino)
            .ToListAsync(cancellationToken);
    }
}
