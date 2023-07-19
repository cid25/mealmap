using Mealmap.Model;


namespace Mealmap.Api.UnitTests
{
    internal class FakeMealRepository : Dictionary<Guid, Meal>, IMealRepository
    {
        public IEnumerable<Meal> GetAll()
        {
            return Values;
        }

        public Meal? GetById(Guid id)
        {
            TryGetValue(id, out var meal);

            return meal;
        }

        public void Create(Meal meal)
        {
            Add((Guid)meal.Id, meal);
        }
    }
}
