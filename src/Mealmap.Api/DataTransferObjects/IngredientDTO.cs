namespace Mealmap.Api.DataTransferObjects;

public record IngredientDTO
{
    /// <summary>
    /// The amount of the ingredient required for creating the base number of servings of the dish.
    /// </summary>
    /// <example>0.5</example>
    public decimal Quantity { get; set; }

    /// <summary>
    /// The unit of measurement to which the quantity refers.
    /// </summary>
    /// <example>Kilogram</example>
    /// <example>Mililiter</example>
    public string UnitOfMeasurement { get; set; }

    /// <summary>
    /// The description of the ingredient.
    /// </summary>
    /// <example>Potatoes</example>
    /// <example>Grated parmesan</example>
    public string Description { get; set; }

    public IngredientDTO(decimal quantity, string unitOfMeasurement, string description)
         => (Quantity, UnitOfMeasurement, Description) = (quantity, unitOfMeasurement, description);
}
