namespace Mealmap.Api.Common;

public interface ICommandProcessor<TCommand, TResponse>
{
    public Task<CommandNotification<TResponse>> Process(TCommand command);
}
