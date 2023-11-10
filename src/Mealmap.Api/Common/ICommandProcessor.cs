namespace Mealmap.Api.Shared;

public interface ICommandProcessor<TCommand, TResponse>
{
    public Task<CommandNotification<TResponse>> Process(TCommand command);
}
