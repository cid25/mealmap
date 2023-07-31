using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mealmap.Api.DataTransferObjects;

public record DishDTO
{
    /// <summary>
    /// The identity of the dish.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? Id { get; init; }

    /// <summary>
    /// The current entity tag for the dish.
    /// </summary>
    /// <example>AAAAAAAAB9E=</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ETag { get; init; }

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
    /// The Url to fetch the dish's image from.
    /// </summary>
    /// <example>https://host.com/api/dishes/3fa85f64-5717-4562-b3fc-2c963f66afa6/image</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ImageUrl { get; set; }

    /// <summary>
    /// The number of servings produced with the stated ingredients.
    /// </summary>
    /// <example>2</example>
    [Range(1, int.MaxValue)]
    public int Servings { get; set; }

    /// <summary>
    /// The ingredients required to prepare the dish.
    /// </summary>
    public IngredientDTO[]? Ingredients { get; set; }

    public DishDTO(string name)
        => Name = name;
}
