using System.ComponentModel.DataAnnotations;

namespace OnibusExpress.Application.Dtos;

public sealed record CriarReservaRequest(
    [Required, MaxLength(160)] string Nome,
    [Required, MaxLength(14)] string Cpf,
    [Required, EmailAddress, MaxLength(180)] string Email,
    [Range(1, int.MaxValue)] int ViagemId,
    [Range(1, int.MaxValue)] int AssentoNumero);
