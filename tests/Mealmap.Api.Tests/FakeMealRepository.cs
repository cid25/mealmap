using Mealmap.Model;


namespace Mealmap.Api.Tests
{
    internal class FakeMealRepository : Dictionary<Guid,Meal>, IMealRepository
    { 
        public IEnumerable<Meal> GetAll()
        {
            return Values;
        }

        public Meal? GetById(Guid id)
        {
            Meal? meal;
            TryGetValue(id, out meal);

            return meal;
        }

        public void Create(Meal meal)
        {
            Add(meal.Id, meal);
        }
    }
}
