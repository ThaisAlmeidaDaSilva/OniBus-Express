using Microsoft.EntityFrameworkCore;
using OnibusExpress.Models;

namespace OnibusExpress.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(OnibusDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Rotas.AnyAsync())
        {
            return;
        }

        var saoPauloRio = new Rota
        {
            Origem = "Sao Paulo",
            Destino = "Rio de Janeiro",
            DuracaoEstimada = TimeSpan.FromHours(6)
        };

        var saoPauloCuritiba = new Rota
        {
            Origem = "Sao Paulo",
            Destino = "Curitiba",
            DuracaoEstimada = TimeSpan.FromHours(6.5)
        };

        dbContext.Rotas.AddRange(saoPauloRio, saoPauloCuritiba);
        dbContext.Viagens.AddRange(
            new Viagem
            {
                Rota = saoPauloRio,
                DataHoraPartida = DateTime.Today.AddDays(1).AddHours(8),
                Preco = 129.90m,
                TotalAssentos = 46
            },
            new Viagem
            {
                Rota = saoPauloRio,
                DataHoraPartida = DateTime.Today.AddDays(1).AddHours(22),
                Preco = 149.90m,
                TotalAssentos = 46
            },
            new Viagem
            {
                Rota = saoPauloCuritiba,
                DataHoraPartida = DateTime.Today.AddDays(2).AddHours(9),
                Preco = 119.90m,
                TotalAssentos = 42
            });

        await dbContext.SaveChangesAsync();
    }
}
