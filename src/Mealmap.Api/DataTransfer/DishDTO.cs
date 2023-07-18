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
        /// <example>An italian-style pizza with pineapple and mozarella.</example>
        public string? Description { get; init; }

        /// <summary>
        /// A short description of the dish.
        /// </summary>
        /// <example>https://host.com/api/dishes/3fa85f64-5717-4562-b3fc-2c963f66afa6/image</example>
        public Uri? ImageUrl { get; set; }

        /// <summary>
        /// The number of servings produced with the stated ingredients.
        /// </summary>
        /// <example>2</example>
        [Range(1, 20)]
        public byte Servings { get; set; }

        /// <summary>
        /// The ingredients required to prepare the dish.
        /// </summary>
        public IngredientDTO[]? Ingredients { get; set; }

        public DishDTO(string name)
            => Name = name;
    }
}
