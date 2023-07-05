using System.ComponentModel.DataAnnotations;

namespace Mealmap.Model
{
    public class Dish
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Dish(string name)
        {
            Name = name;
        }
    }
}
