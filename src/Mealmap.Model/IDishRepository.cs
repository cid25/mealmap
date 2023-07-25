namespace Mealmap.Model
{
    public interface IDishRepository
    {
        public IEnumerable<Dish> GetAll();

        public Dish? GetSingle(Guid id);

        public void Add(Dish dish);

        public void Update(Dish dish, bool retainImage);

        public void Remove(Dish dish);
    }
}
