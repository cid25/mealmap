using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("dishes", Schema = "mealmap")]
    public class Dish
    {
        public Guid Id { get; init; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DishImage? Image { get; set; }

        [Range(1, 20)]
        public byte Servings { get; set; }

        public List<Ingredient>? Ingredients { get; set; }

        public Dish(string name)
        {
            Name = name;
        }
        
        public void AddIngredient(decimal quantity, string unitOfMeasurementName, string description)
        {
            var unit = new UnitOfMeasurement(unitOfMeasurementName);
            Ingredient ingredient = new(quantity, unit, description);

            if (Ingredients is null)
                Ingredients = new List<Ingredient>();

            Ingredients.Add(ingredient);
        }

        public void RemoveIngredient(decimal quantity, string unitOfMeasurementName, string description)
        {
            if (Ingredients is null)
                return;

            var unit = new UnitOfMeasurement(unitOfMeasurementName);
            Ingredient newIngredient = new(quantity, unit, description);

            Ingredients.Remove(newIngredient);
        }
    }
}
