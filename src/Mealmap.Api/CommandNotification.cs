namespace Mealmap.Api;

public class CommandNotification<TResponse>
{
    public List<CommandError> Errors { get; } = new();

    public bool Success
    {
        get => !Errors.Any();
    }

    public TResponse? Result { get; set; }
}

public class CommandError
{
    public CommandErrorCodes ErrorCode { get; }

    public string Message { get; } = "";

    public CommandError(CommandErrorCodes errorCode, string message)
        => (ErrorCode, Message) = (errorCode, message);
}

public enum CommandErrorCodes
{
    NotFound,
    NotValid
}
