namespace Mealmap.Api.Common;

public interface IRequestContext
{
    string Scheme { get; }
    string Host { get; }
    int Port { get; }
    string Method { get; }
    string? IfMatchHeader { get; }
}
