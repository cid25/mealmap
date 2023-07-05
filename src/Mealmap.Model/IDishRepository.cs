namespace Mealmap.Model
{
    public interface IDishRepository
    {
        public IEnumerable<Dish> GetAll();
    }
}
