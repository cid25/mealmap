namespace Mealmap.Model
{
    public interface IDishRepository
    {
        public IEnumerable<Dish> GetAll();

        public Dish? GetById(Guid id);

        public void Create(Dish dish);

        public void Update(Dish dish);
    }
}
