using Microsoft.EntityFrameworkCore;
using OnibusExpress.Models;

namespace OnibusExpress.Data;

public sealed class OnibusDbContext(DbContextOptions<OnibusDbContext> options) : DbContext(options)
{
    public DbSet<Rota> Rotas => Set<Rota>();
    public DbSet<Viagem> Viagens => Set<Viagem>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rota>(entity =>
        {
            entity.Property(rota => rota.Origem).HasMaxLength(120).IsRequired();
            entity.Property(rota => rota.Destino).HasMaxLength(120).IsRequired();
            entity.HasIndex(rota => new { rota.Origem, rota.Destino });
        });

        modelBuilder.Entity<Viagem>(entity =>
        {
            entity.Property(viagem => viagem.Preco).HasPrecision(10, 2);
            entity.Property(viagem => viagem.TotalAssentos).HasDefaultValue(46);
            entity.HasOne(viagem => viagem.Rota)
                .WithMany(rota => rota.Viagens)
                .HasForeignKey(viagem => viagem.RotaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.Property(reserva => reserva.Codigo).HasMaxLength(12).IsRequired();
            entity.Property(reserva => reserva.Nome).HasMaxLength(160).IsRequired();
            entity.Property(reserva => reserva.Cpf).HasMaxLength(14).IsRequired();
            entity.Property(reserva => reserva.Email).HasMaxLength(180).IsRequired();
            entity.HasIndex(reserva => reserva.Codigo).IsUnique();
            entity.HasIndex(reserva => new { reserva.ViagemId, reserva.AssentoNumero })
                .IsUnique()
                .HasFilter("[CanceladaEm] IS NULL");
        });
    }
}
