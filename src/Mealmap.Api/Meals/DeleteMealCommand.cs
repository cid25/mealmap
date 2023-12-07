namespace Mealmap.Api.Dishes;

public class DeleteMealCommand(Guid id)
{
    public Guid Id { get; init; } = id;
}
