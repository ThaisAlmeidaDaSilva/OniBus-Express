namespace OnibusExpress.Domain.Entities;

public sealed class Viagem
{
    public int Id { get; set; }
    public int RotaId { get; set; }
    public Rota? Rota { get; set; }
    public DateTime DataHoraPartida { get; set; }
    public decimal Preco { get; set; }
    public int TotalAssentos { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = [];
}
