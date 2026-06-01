namespace OnibusExpress.Domain.Contracts;

public interface IClock
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
