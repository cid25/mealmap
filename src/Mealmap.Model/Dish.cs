using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("dishes", Schema = "mealmap")]
    public class Dish
    {
        public Guid Id { get; init; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public Dish(string name)
        {
            Name = name;
        }
    }
}
