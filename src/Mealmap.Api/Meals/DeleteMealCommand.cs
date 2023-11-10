namespace Mealmap.Api.Dishes;

public class DeleteMealCommand
{
    public Guid Id { get; init; }

    public DeleteMealCommand(Guid id)
        => Id = id;
}
