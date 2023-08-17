namespace Mealmap.Domain.Common.DataAccess;

public class ConcurrentUpdateException : Exception
{
    public ConcurrentUpdateException()
        : base("An entity has been updated concurrently.") { }

    public ConcurrentUpdateException(string message) : base(message) { }

    public ConcurrentUpdateException(string message, Exception inner) : base(message, inner) { }
}
