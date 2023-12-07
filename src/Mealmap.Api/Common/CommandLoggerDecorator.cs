namespace Mealmap.Api.Common;

public class CommandLoggerDecorator<TCommand, TResponse>(ILogger<CommandLoggerDecorator<TCommand, TResponse>> logger, ICommandProcessor<TCommand, TResponse> innerProcessor) : ICommandProcessor<TCommand, TResponse>
    where TCommand : class
{
    public async Task<CommandNotification<TResponse>> Process(TCommand command)
    {
        logger.LogInformation("Processing command: {CommandType} - ({@Command})",
            command.GetType(),
            command);

        return await innerProcessor.Process(command);
    }
}
