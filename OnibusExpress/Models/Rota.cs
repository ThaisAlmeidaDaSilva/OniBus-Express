namespace OnibusExpress.Models;

public sealed class Rota
{
    public int Id { get; set; }
    public required string Origem { get; set; }
    public required string Destino { get; set; }
    public TimeSpan DuracaoEstimada { get; set; }
    public ICollection<Viagem> Viagens { get; set; } = [];
}
