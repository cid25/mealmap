namespace Mealmap.Api.Dishes;

public class DeleteDishImageCommand
{
    public Guid Id { get; init; }

    public DeleteDishImageCommand(Guid id)
    {
        Id = id;
    }
}
