namespace Mealmap.Api.Shared;

public class CommandLoggerDecorator<TCommand, TResponse> : ICommandProcessor<TCommand, TResponse>
    where TCommand : class
{
    private readonly ILogger<CommandLoggerDecorator<TCommand, TResponse>> _logger;
    private readonly ICommandProcessor<TCommand, TResponse> _innerProcessor;

    public CommandLoggerDecorator(ILogger<CommandLoggerDecorator<TCommand, TResponse>> logger, ICommandProcessor<TCommand, TResponse> innerProcessor)
    {
        _logger = logger;
        _innerProcessor = innerProcessor;
    }

    public async Task<CommandNotification<TResponse>> Process(TCommand command)
    {
        _logger.LogInformation("Processing command: {CommandType} - ({@Command})",
            command.GetType(),
            command);

        return await _innerProcessor.Process(command);
    }
}
