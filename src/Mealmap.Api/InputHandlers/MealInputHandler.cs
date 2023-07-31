﻿using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.Exceptions;
using Mealmap.Domain.Exceptions;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.InputMappers;

public class MealInputHandler : IInputHandler<Meal, MealDTO>
{
    private readonly MealService _mealService;

    public MealInputHandler(MealService mealService)
    {
        _mealService = mealService;
    }


    /// <exception cref="ValidationException"></exception>
    /// <exception cref="DomainValidationException"></exception>
    public Meal FromDataTransferObject(MealDTO dto)
    {
        if (dto.Id != Guid.Empty && dto.Id != null)
        {
            throw new ValidationException("Id not allowed as part of request.");
        }

        var meal = _mealService.CreateMeal(Guid.NewGuid(), dto.DiningDate);

        if (dto.Courses != null)
        {
            foreach (var course in dto.Courses)
            {
                _mealService.AddCourseToMeal(meal, course.Index, course.MainCourse, course.DishId);
            }
        }

        return meal;
    }
}