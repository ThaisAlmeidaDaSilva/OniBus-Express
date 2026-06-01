namespace OnibusExpress.Dtos;

public sealed record ViagemDetalheDto(
    int Id,
    string Origem,
    string Destino,
    DateTime DataHoraPartida,
    decimal Preco,
    int TotalAssentos,
    IReadOnlyCollection<int> AssentosLivres,
    IReadOnlyCollection<int> AssentosOcupados);
