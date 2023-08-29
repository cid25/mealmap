using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class CommandLogger<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<CommandLogger<TRequest, TResponse>> _logger;

    public CommandLogger(ILogger<CommandLogger<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing command: {CommandType} - ({@Command})",
            request.GetType(),
            request);

        return await next();
    }
}
