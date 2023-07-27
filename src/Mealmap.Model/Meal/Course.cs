using System.ComponentModel.DataAnnotations;

namespace Mealmap.Domain.MealAggregate;

public record Course
{
    [Range(1, int.MaxValue)]
    public int Index { get; internal init; }

    public bool MainCourse { get; internal init; }

    public Guid DishId { get; internal init; }

    internal Course(int index, bool mainCourse, Guid dishId)
        => (Index, MainCourse, DishId) = (index, mainCourse, dishId);
}
