using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransfer
{
    public record MealDTO
    {
        public Guid? Id { get; init; }

        /// <summary>
        /// The date the meal is taking place.
        /// </summary>
        /// <example>2020-12-31</example>
        [Required]
        public DateOnly Date { get; init; }

        /// <summary>
        /// The dish served at the meal.
        /// </summary>
        public Guid? Dish { get; init; }
    }
}
