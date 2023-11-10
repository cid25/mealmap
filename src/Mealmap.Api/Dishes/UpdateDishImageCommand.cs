namespace Mealmap.Api.Dishes;

public class UpdateDishImageCommand
{
    public Guid Id { get; init; }
    public Image Image { get; init; }

    public UpdateDishImageCommand(Guid id, Image image)
    {
        Id = id;
        Image = image;
    }
}
