namespace OnibusExpress.Application.Results;

public sealed record ApplicationResult(ApplicationStatus Status, string? Error = null)
{
    public static ApplicationResult Success() => new(ApplicationStatus.Success);
    public static ApplicationResult NotFound(string? error = null) => new(ApplicationStatus.NotFound, error);
    public static ApplicationResult BadRequest(string error) => new(ApplicationStatus.BadRequest, error);
}
