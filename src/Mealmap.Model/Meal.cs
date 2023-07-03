using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Model
{
    [Table("meals", Schema = "mealmap")]
    public class Meal
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }

        public Meal(string name)
        {
            Name = name;
        }   
    }
}
