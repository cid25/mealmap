using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("dishes", Schema = "mealmap")]
    public class Dish
    {
        public Guid Id { get; init; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
                
        public string? Description { get; set; }

        public DishImage? Image { get; set; }

        public List<Ingredient>? Ingredients { get; set; }

        public Dish(string name)
        {
            Name = name;
        }
        
        public void AddIngredient(decimal quantity, string unitOfMeasurementName, string description)
        {
            Ingredient ingredient = new(quantity, unitOfMeasurementName, description);

            if (Ingredients is null)
                Ingredients = new List<Ingredient>() { ingredient };
        }

        public void RemoveIngredient(decimal quantity, string unitOfMeasurementName, string description)
        {
            if (Ingredients is null)
                return;
            
            Ingredient newIngredient = new(quantity, unitOfMeasurementName, description);

            foreach (var ingredient in Ingredients)
            {
                if (ingredient.Equals(newIngredient))
                {
                    Ingredients.Remove(ingredient);
                    break;
                }
            }
        }
    }
}
