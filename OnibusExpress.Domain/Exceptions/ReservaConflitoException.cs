namespace OnibusExpress.Domain.Exceptions;

public sealed class ReservaConflitoException : Exception
{
    public ReservaConflitoException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
