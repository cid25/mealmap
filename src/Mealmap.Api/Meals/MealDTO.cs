﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mealmap.Api.Meals;

public record MealDTO
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? Id { get; init; }

    /// <summary>
    /// The current entity tag for the dish.
    /// </summary>
    /// <example>AAAAAAAAB9E=</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ETag { get; init; }

    /// <summary>
    /// The date the meal is taking place.
    /// </summary>
    /// <example>2020-12-31</example>
    [Required]
    public DateOnly DiningDate { get; init; }

    /// <summary>
    /// The courses served at the meal.
    /// </summary>
    public CourseDTO[]? Courses { get; init; }
}
