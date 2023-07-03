using Mealmap.Model;


namespace Mealmap.Api.UnitTests
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
            if (meal.Id == null)
                throw new ArgumentNullException(nameof(meal.Id));
            
            Add((Guid) meal.Id, meal);
        }
    }
}
