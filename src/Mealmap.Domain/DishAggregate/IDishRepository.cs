using Mealmap.Domain.Common.DataAccess;

namespace Mealmap.Domain.DishAggregate;

public interface IDishRepository : IRepository<Dish>
{
    public IEnumerable<Dish> GetAll();
}
