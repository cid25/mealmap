namespace Mealmap.Model
{
    public class Meal
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        
        public Meal(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
