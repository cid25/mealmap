namespace Mealmap.Domain.Common.Validation;

public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    { }

    public DomainValidationException(string message, Exception inner) : base(message, inner)
    { }
}
