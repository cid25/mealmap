using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.Meals;

public record CourseDTO
{
    /// <summary>
    /// The index of the course in the order of courses of the meal.
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int Index { get; init; }

    /// <summary>
    /// The id of the dish served in this course of the meal.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid DishId { get; init; }

    /// <summary>
    /// Whether or not this is the main course of the meal.
    /// </summary>
    /// <example>true</example>
    public bool MainCourse { get; init; }

    /// <summary>
    /// The number of people served this course.
    /// </summary>
    /// <example>true</example>
    [Range(1, int.MaxValue)]
    public int Attendees { get; init; }
}
