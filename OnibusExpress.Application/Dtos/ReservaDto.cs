namespace OnibusExpress.Application.Dtos;

public sealed record ReservaDto(
    string Codigo,
    string Nome,
    string Cpf,
    string Email,
    int ViagemId,
    string Origem,
    string Destino,
    DateTime DataHoraPartida,
    int AssentoNumero,
    DateTime CriadaEm,
    DateTime? CanceladaEm,
    string Status);
