using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransfer
{
    public record DishDTO
    {
        public Guid? Id { get; init; }

        /// <summary>
        /// The name of the dish.
        /// </summary>
        /// <example>Pineapple Pizza</example>
        [Required]
        public string Name { get; init; }

        /// <summary>
        /// A short description of the dish.
        /// </summary>
        /// <example>An italian-style Pizza with Pineapple and Mozarella.</example>
        public string? Description { get; init; }

        public DishDTO(string name)
        {
            Name = name;
        }
    }
}
