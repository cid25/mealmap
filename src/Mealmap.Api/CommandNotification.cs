namespace Mealmap.Api;

public class CommandNotification<TResponse>
{
    public List<CommandError> Errors { get; } = new();

    public bool Succeeded { get => !Errors.Any(); }

    public bool Failed { get => Errors.Any(); }

    public TResponse? Result { get; set; }

    public CommandNotification<TResponse> WithError(CommandError theError)
    {
        Errors.Add(theError);
        return this;
    }

    public CommandNotification<TResponse> WithError(CommandErrorCodes errorcode, string message)
    {
        return WithError(new CommandError(errorcode, message));
    }

    public CommandNotification<TResponse> WithErrors(IEnumerable<CommandError> theErrors)
    {
        Errors.AddRange(theErrors);
        return this;
    }

    public CommandNotification<TResponse> WithValidationError(string message)
    {
        return WithError(CommandErrorCodes.NotValid, message);
    }

    public CommandNotification<TResponse> WithNotFoundError(string message)
    {
        return WithError(CommandErrorCodes.NotFound, message);
    }

    public CommandNotification<TResponse> WithVersionMismatchError(string message
        = "If-Match Header does not match existing version.")
    {
        return WithError(CommandErrorCodes.VersionMismatch, message);
    }
}
