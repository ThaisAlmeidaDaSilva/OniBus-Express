namespace OnibusExpress.Dtos;

public sealed record RotaDto(
    int Id,
    string Origem,
    string Destino,
    string DuracaoEstimada);
