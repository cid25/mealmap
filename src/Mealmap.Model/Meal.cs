using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("meals", Schema = "mealmap")]
    public class Meal
    {
        public Guid Id { get; init; }

        public DateOnly DiningDate { get; set; }

        public Dish? Dish { get; set; }
    }
}
