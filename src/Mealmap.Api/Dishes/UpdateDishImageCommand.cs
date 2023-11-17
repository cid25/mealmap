namespace Mealmap.Api.Dishes;

public class UpdateDishImageCommand(Guid id, Image image)
{
    public Guid Id { get; init; } = id;
    public Image Image { get; init; } = image;
}
