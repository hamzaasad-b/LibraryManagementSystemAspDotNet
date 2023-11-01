namespace Domain.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}