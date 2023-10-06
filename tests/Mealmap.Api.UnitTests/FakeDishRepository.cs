using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.UnitTests;

internal class FakeDishRepository : Dictionary<Guid, Dish>, IRepository<Dish>
{
    public IEnumerable<Dish> GetAll()
    {
        return Values;
    }

    public Dish? GetSingleById(Guid id)
    {
        TryGetValue(id, out var dish);

        return dish;
    }

    public void Add(Dish dish)
    {
        Add((Guid)dish.Id, dish);
    }

    public void Update(Dish dish)
    {
        if (!Remove(dish.Id))
            throw new InvalidOperationException();

        Add(dish.Id, dish);
    }

    public void Remove(Dish dish)
    {
        if (!Remove(dish.Id))
            throw new InvalidOperationException();
    }
}
