namespace OnibusExpress.Application.Dtos;

public sealed record ViagemResumoDto(
    int Id,
    int RotaId,
    string Origem,
    string Destino,
    DateTime DataHoraPartida,
    decimal Preco,
    int TotalAssentos,
    int AssentosDisponiveis);
