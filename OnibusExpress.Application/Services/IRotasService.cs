using OnibusExpress.Application.Dtos;

namespace OnibusExpress.Application.Services;

public interface IRotasService
{
    Task<IReadOnlyCollection<RotaDto>> ListarAsync(CancellationToken cancellationToken = default);
}
