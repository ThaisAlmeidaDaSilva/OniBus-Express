using Microsoft.EntityFrameworkCore;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Contracts;
using OnibusExpress.Infrastructure;
using OnibusExpress.Infrastructure.Data;
using OnibusExpress.Infrastructure.Repositories;
using OnibusExpress.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<OnibusDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRotasService, RotasService>();
builder.Services.AddScoped<IViagensService, ViagensService>();
builder.Services.AddScoped<IReservasService, ReservasService>();
builder.Services.AddScoped<IRotaRepository, RotaRepository>();
builder.Services.AddScoped<IViagemRepository, ViagemRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/", () => Results.Redirect("/swagger"));
    app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
    app.MapGet("/swagger/index.html", () => Results.Content(GetSwaggerUiHtml(), "text/html"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OnibusDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

app.Run();

static string GetSwaggerUiHtml()
{
    return """
        <!doctype html>
        <html lang="pt-BR">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>OnibusExpress API - Swagger</title>
          <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/swagger-ui-dist@5/swagger-ui.css" />
          <style>
            body { margin: 0; background: #fafafa; }
            .swagger-ui .topbar { display: none; }
          </style>
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://cdn.jsdelivr.net/npm/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
          <script>
            window.onload = () => {
              window.ui = SwaggerUIBundle({
                url: '/openapi/v1.json',
                dom_id: '#swagger-ui',
                deepLinking: true,
                presets: [
                  SwaggerUIBundle.presets.apis
                ]
              });
            };
          </script>
        </body>
        </html>
        """;
}
