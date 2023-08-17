namespace Mealmap.Api
{
    public interface ICommandValidator<TCommand>
        where TCommand : class
    {
        IEnumerable<CommandError> Validate(TCommand command);
    }
}
