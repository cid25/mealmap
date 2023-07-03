﻿using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransferObjects
{
    public record MealDto
    {
        public Guid? Id { get; init; }

        [Required]
        public string Name { get; init; }

        public MealDto(string name)
        {
            Name = name;
        }
    }
}
