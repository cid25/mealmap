using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("meals", Schema = "mealmap")]
    public class Meal
    {
        public Guid Id { get; init; }

        public DateOnly Date { get; set; }

        public Dish? Dish { get; set; }
    }
}
