using Mealmap.Model;


namespace Mealmap.Api.UnitTests
{
    internal class FakeDishRepository : Dictionary<Guid,Dish>, IDishRepository
    { 
        public IEnumerable<Dish> GetAll()
        {
            return Values;
        }

        public Dish? GetById(Guid id)
        {
            Dish? dish;
            TryGetValue(id, out dish);

            return dish;
        }

        public void Create(Dish dish)
        {
            if (dish.Id == null)
                throw new ArgumentNullException(nameof(dish.Id));

            Add((Guid)dish.Id, dish);
        }
    }
}
