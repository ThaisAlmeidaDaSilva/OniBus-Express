using OnibusExpress.Application.Dtos;
using OnibusExpress.Application.Results;
using OnibusExpress.Application.Validation;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;

namespace OnibusExpress.Application.Services;

public sealed class ReservasService(
    IViagemRepository viagemRepository,
    IReservaRepository reservaRepository,
    IClock clock) : IReservasService
{
    public async Task<ApplicationResult<ReservaDto>> CriarAsync(
        CriarReservaRequest request,
        CancellationToken cancellationToken = default)
    {
        var viagem = await viagemRepository.ObterComReservasAsync(request.ViagemId, cancellationToken);

        if (viagem is null || viagem.Rota is null)
        {
            return ApplicationResult<ReservaDto>.NotFound("Viagem nao encontrada.");
        }

        if (viagem.DataHoraPartida <= clock.Now)
        {
            return ApplicationResult<ReservaDto>.BadRequest("Nao e possivel reservar passagem para uma viagem ja realizada.");
        }

        if (!CpfValidator.IsValid(request.Cpf))
        {
            return ApplicationResult<ReservaDto>.BadRequest("CPF invalido. Informe 11 digitos validos ou o formato 000.000.000-00.");
        }

        if (request.AssentoNumero > viagem.TotalAssentos)
        {
            return ApplicationResult<ReservaDto>.BadRequest($"O assento deve estar entre 1 e {viagem.TotalAssentos}.");
        }

        var assentoOcupado = await reservaRepository.AssentoOcupadoAsync(
            request.ViagemId,
            request.AssentoNumero,
            cancellationToken);

        if (assentoOcupado)
        {
            return ApplicationResult<ReservaDto>.Conflict("Assento ja esta ocupado para esta viagem.");
        }

        var reserva = new Reserva
        {
            Codigo = await GerarCodigoUnicoAsync(cancellationToken),
            Nome = request.Nome.Trim(),
            Cpf = CpfValidator.Format(request.Cpf),
            Email = request.Email.Trim(),
            ViagemId = request.ViagemId,
            Viagem = viagem,
            AssentoNumero = request.AssentoNumero,
            CriadaEm = clock.UtcNow
        };

        await reservaRepository.AdicionarAsync(reserva, cancellationToken);

        try
        {
            await reservaRepository.SalvarAsync(cancellationToken);
        }
        catch (ReservaConflitoException)
        {
            return ApplicationResult<ReservaDto>.Conflict("Nao foi possivel concluir a reserva. Verifique se o assento continua disponivel.");
        }

        return ApplicationResult<ReservaDto>.Created(Mapear(reserva));
    }

    public async Task<ApplicationResult<ReservaDto>> ConsultarAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        var reserva = await reservaRepository.BuscarPorCodigoAsync(codigo, cancellationToken);

        return reserva is null
            ? ApplicationResult<ReservaDto>.NotFound()
            : ApplicationResult<ReservaDto>.Success(Mapear(reserva));
    }

    public async Task<ApplicationResult> CancelarAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        var reserva = await reservaRepository.BuscarPorCodigoAsync(codigo, cancellationToken);

        if (reserva is null)
        {
            return ApplicationResult.NotFound();
        }

        if (reserva.CanceladaEm is not null)
        {
            return ApplicationResult.Success();
        }

        if (reserva.Viagem is null || clock.Now > reserva.Viagem.DataHoraPartida.AddHours(-2))
        {
            return ApplicationResult.BadRequest("Cancelamento permitido somente ate 2 horas antes da partida.");
        }

        reserva.CanceladaEm = clock.UtcNow;
        await reservaRepository.SalvarAsync(cancellationToken);

        return ApplicationResult.Success();
    }

    private async Task<string> GerarCodigoUnicoAsync(CancellationToken cancellationToken)
    {
        string codigo;

        do
        {
            codigo = $"{GerarLetras(3)}-{Random.Shared.Next(10000, 100000)}";
        }
        while (await reservaRepository.CodigoExisteAsync(codigo, cancellationToken));

        return codigo;
    }

    private static string GerarLetras(int quantidade)
    {
        const string letras = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        return new string(Enumerable
            .Range(0, quantidade)
            .Select(_ => letras[Random.Shared.Next(letras.Length)])
            .ToArray());
    }

    private static ReservaDto Mapear(Reserva reserva)
    {
        var viagem = reserva.Viagem;
        var rota = viagem?.Rota;

        return new ReservaDto(
            reserva.Codigo,
            reserva.Nome,
            reserva.Cpf,
            reserva.Email,
            reserva.ViagemId,
            rota?.Origem ?? string.Empty,
            rota?.Destino ?? string.Empty,
            viagem?.DataHoraPartida ?? default,
            reserva.AssentoNumero,
            reserva.CriadaEm,
            reserva.CanceladaEm,
            reserva.EstaAtiva ? "Ativa" : "Cancelada");
    }
}
