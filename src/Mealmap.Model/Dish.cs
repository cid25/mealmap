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

        public Dish(string name)
        {
            Name = name;
        }
    }
}
