using OnibusExpress.Application.Dtos;
using OnibusExpress.Domain.Contracts;

namespace OnibusExpress.Application.Services;

public sealed class RotasService(IRotaRepository rotaRepository) : IRotasService
{
    public async Task<IReadOnlyCollection<RotaDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var rotas = await rotaRepository.ListarAsync(cancellationToken);

        return rotas
            .Select(rota => new RotaDto(
                rota.Id,
                rota.Origem,
                rota.Destino,
                rota.DuracaoEstimada.ToString(@"hh\:mm")))
            .ToArray();
    }
}
