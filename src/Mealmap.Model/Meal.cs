namespace Mealmap.Model
{
    public class Meal
    {
        public string Name { get; init; }
        
        public Meal(string name)
        {
            Name = name;
        }
    }
}