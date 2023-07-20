using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mealmap.Api.DataTransfer
{
    public record MealDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? Id { get; init; }

        /// <summary>
        /// The date the meal is taking place.
        /// </summary>
        /// <example>2020-12-31</example>
        [Required]
        public DateOnly DiningDate { get; init; }

        /// <summary>
        /// The dish served at the meal.
        /// </summary>
        public Guid? DishId { get; init; }
    }
}
