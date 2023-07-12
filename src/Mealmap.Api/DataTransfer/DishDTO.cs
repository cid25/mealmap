using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransfer
{
    public record DishDTO
    {
        /// <summary>
        /// The identity of the dish.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid? Id { get; init; }

        /// <summary>
        /// The name of the dish.
        /// </summary>
        /// <example>Pineapple Pizza</example>
        [Required]
        [MaxLength(100)]
        public string Name { get; init; }

        /// <summary>
        /// A short description of the dish.
        /// </summary>
        /// <example>An italian-style Pizza with Pineapple and Mozarella.</example>
        public string? Description { get; init; }

        /// <summary>
        /// A short description of the dish.
        /// </summary>
        /// <example>https://host.com/api/dishes/3fa85f64-5717-4562-b3fc-2c963f66afa6/image</example>
        public Uri? ImageUrl { get; set; }

        public DishDTO(string name)
        {
            Name = name;
        }
    }
}
