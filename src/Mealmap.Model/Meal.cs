namespace Mealmap.Model
{
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
