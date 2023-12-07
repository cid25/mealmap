namespace Mealmap.Api.Dishes;

public class DeleteDishCommand(Guid id)
{
    public Guid Id { get; init; } = id;
}
