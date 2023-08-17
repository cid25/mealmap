namespace Mealmap.Api
{
    public interface ICommandValidator<TCommand>
        where TCommand : class
    {
        IReadOnlyCollection<CommandError> Validate(TCommand command);
    }
}
