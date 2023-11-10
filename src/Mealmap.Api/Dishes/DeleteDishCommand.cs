namespace Mealmap.Api.Dishes;

public class DeleteDishCommand
{
    public Guid Id { get; init; }

    public DeleteDishCommand(Guid id)
        => Id = id;
}
