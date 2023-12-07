namespace Mealmap.Api.Dishes;

public class DeleteDishImageCommand(Guid id)
{
    public Guid Id { get; init; } = id;
}
