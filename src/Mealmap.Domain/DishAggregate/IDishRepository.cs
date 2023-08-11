using Mealmap.Domain.Common;

namespace Mealmap.Domain.DishAggregate;

public interface IDishRepository : IRepository<Dish>
{
    public IEnumerable<Dish> GetAll();
}
