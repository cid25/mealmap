﻿using System.ComponentModel.DataAnnotations;

namespace Mealmap.Domain.MealAggregate;

public record Course
{
#pragma warning disable IDE0052
    private readonly Guid Id;
#pragma warning restore IDE0052

    [Range(1, int.MaxValue)]
    public int Index { get; internal init; }

    public bool MainCourse { get; internal init; }

    public Guid DishId { get; internal init; }

    internal Course(int index, bool mainCourse, Guid dishId)
    {
        Id = Guid.NewGuid();
        (Index, MainCourse, DishId) = (index, mainCourse, dishId);
    }
}