namespace OnibusExpress.Domain.Entities;

public sealed class Reserva
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Nome { get; set; }
    public required string Cpf { get; set; }
    public required string Email { get; set; }
    public int ViagemId { get; set; }
    public Viagem? Viagem { get; set; }
    public int AssentoNumero { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime? CanceladaEm { get; set; }
    public bool EstaAtiva => CanceladaEm is null;
}
