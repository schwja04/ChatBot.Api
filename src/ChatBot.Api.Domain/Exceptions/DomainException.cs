namespace ChatBot.Api.Domain.Exceptions;

public abstract class DomainException : ApplicationException
{
    protected DomainException()
    {
    }
    
    protected DomainException(string message) : base(message)
    {
    }
    
    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}