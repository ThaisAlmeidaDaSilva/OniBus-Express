using OnibusExpress.Domain.Contracts;

namespace OnibusExpress.Infrastructure.Services;

public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
