namespace Mealmap.Api.Shared;

public interface IRequestContext
{
    string Scheme { get; }
    string Host { get; }
    int Port { get; }
    string Method { get; }
    string? IfMatchHeader { get; }
}
