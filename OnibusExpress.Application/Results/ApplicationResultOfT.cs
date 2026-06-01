namespace OnibusExpress.Application.Results;

public sealed record ApplicationResult<T>(ApplicationStatus Status, T? Value = default, string? Error = null)
{
    public static ApplicationResult<T> Success(T value) => new(ApplicationStatus.Success, value);
    public static ApplicationResult<T> Created(T value) => new(ApplicationStatus.Created, value);
    public static ApplicationResult<T> NotFound(string? error = null) => new(ApplicationStatus.NotFound, default, error);
    public static ApplicationResult<T> BadRequest(string error) => new(ApplicationStatus.BadRequest, default, error);
    public static ApplicationResult<T> Conflict(string error) => new(ApplicationStatus.Conflict, default, error);
}
