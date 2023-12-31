﻿namespace Mealmap.Api.Common;

public class CommandError
{
    public CommandErrorCodes ErrorCode { get; }

    public string Message { get; } = "";

    public CommandError(CommandErrorCodes errorCode)
        => ErrorCode = errorCode;

    public CommandError(CommandErrorCodes errorCode, string message)
        => (ErrorCode, Message) = (errorCode, message);
}

public enum CommandErrorCodes
{
    NotValid,
    NotFound,
    VersionMismatch,
}
