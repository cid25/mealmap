using Mealmap.Model;


namespace Mealmap.Api.UnitTests
{
    internal class FakeMealRepository : Dictionary<Guid, Meal>, IMealRepository
    {
        public IEnumerable<Meal> GetMultiple(DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            return Values
                .Where(v => v.DiningDate >= (fromDate ?? new DateOnly(1990, 1, 1))
                    && v.DiningDate <= (toDate ?? new DateOnly(2999, 12, 31)));
        }

        public Meal? GetSingle(Guid id)
        {
            TryGetValue(id, out var meal);

            return meal;
        }

        public void Add(Meal meal)
        {
            Add((Guid)meal.Id, meal);
        }

        public void Remove(Meal meal)
        {
            Remove(meal.Id);
        }
    }
}
