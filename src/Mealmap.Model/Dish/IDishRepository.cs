namespace Mealmap.Domain.DishAggregate;

public interface IDishRepository
{
    public IEnumerable<Dish> GetAll();

    public Dish? GetSingleById(Guid id);

    public void Add(Dish dish);

    //public void Update(Dish dish, bool retainImage);
    public void Update(Dish dish);

    public void Remove(Dish dish);
}
