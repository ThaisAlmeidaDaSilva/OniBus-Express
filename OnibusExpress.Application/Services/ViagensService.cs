using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;
using OnibusExpress.Domain.Contracts;

namespace OnibusExpress.Application.Services;

public sealed class ViagensService(IViagemRepository viagemRepository) : IViagensService
{
    public async Task<ApplicationResult<IReadOnlyCollection<ViagemResumoDto>>> BuscarAsync(
        string origem,
        string destino,
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(destino))
        {
            return ApplicationResult<IReadOnlyCollection<ViagemResumoDto>>.BadRequest("Informe origem, destino e data.");
        }

        var viagens = await viagemRepository.BuscarAsync(origem, destino, data, cancellationToken);

        var response = viagens
            .Select(viagem => new ViagemResumoDto(
                viagem.Id,
                viagem.RotaId,
                viagem.Rota?.Origem ?? string.Empty,
                viagem.Rota?.Destino ?? string.Empty,
                viagem.DataHoraPartida,
                viagem.Preco,
                viagem.TotalAssentos,
                viagem.TotalAssentos - viagem.Reservas.Count(reserva => reserva.CanceladaEm == null)))
            .ToArray();

        return ApplicationResult<IReadOnlyCollection<ViagemResumoDto>>.Success(response);
    }

    public async Task<ApplicationResult<ViagemDetalheDto>> DetalharAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var viagem = await viagemRepository.ObterComReservasAsync(id, cancellationToken);

        if (viagem is null || viagem.Rota is null)
        {
            return ApplicationResult<ViagemDetalheDto>.NotFound();
        }

        var ocupados = viagem.Reservas
            .Where(reserva => reserva.CanceladaEm is null)
            .Select(reserva => reserva.AssentoNumero)
            .Order()
            .ToArray();

        var livres = Enumerable.Range(1, viagem.TotalAssentos)
            .Except(ocupados)
            .ToArray();

        var response = new ViagemDetalheDto(
            viagem.Id,
            viagem.Rota.Origem,
            viagem.Rota.Destino,
            viagem.DataHoraPartida,
            viagem.Preco,
            viagem.TotalAssentos,
            livres,
            ocupados);

        return ApplicationResult<ViagemDetalheDto>.Success(response);
    }
}
