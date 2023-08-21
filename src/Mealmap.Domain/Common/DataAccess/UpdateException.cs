namespace Mealmap.Domain.Common.DataAccess;

public class UpdateException : Exception
{
    public UpdateException()
        : base("An entity has been updated concurrently.") { }

    public UpdateException(string message) : base(message) { }

    public UpdateException(string message, Exception inner) : base(message, inner) { }
}
