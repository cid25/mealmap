using Mealmap.Model;


namespace Mealmap.Api.UnitTests
{
    internal class FakeDishRepository : Dictionary<Guid, Dish>, IDishRepository
    {
        public IEnumerable<Dish> GetAll()
        {
            return Values;
        }

        public Dish? GetSingle(Guid id)
        {
            TryGetValue(id, out var dish);

            return dish;
        }

        public void Add(Dish dish)
        {
            Add((Guid)dish.Id, dish);
        }

        public void Update(Dish dish, bool retainImage)
        {
            var oldDish = GetSingle(dish.Id);

            if (!Remove(dish.Id))
                throw new InvalidOperationException();

            if (retainImage && oldDish!.Image != null)
                dish.Image = oldDish.Image;

            Add(dish.Id, dish);
        }

        public void Remove(Dish dish)
        {
            if (!Remove(dish.Id))
                throw new InvalidOperationException();
        }
    }
}
