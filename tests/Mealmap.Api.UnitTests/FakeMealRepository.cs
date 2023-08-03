namespace Mealmap.Api.UnitTests;

using Mealmap.Domain.MealAggregate;

internal class FakeMealRepository : Dictionary<Guid, Meal>, IMealRepository
{
    public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        return Values
            .Where(v => v.DiningDate >= (fromDate ?? new DateOnly(1990, 1, 1))
                && v.DiningDate <= (toDate ?? new DateOnly(2999, 12, 31)));
    }

    public Meal? GetSingleById(Guid id)
    {
        TryGetValue(id, out var meal);

        return meal;
    }

    public void Add(Meal meal)
    {
        Add((Guid)meal.Id, meal);
    }

    public void Update(Meal meal)
    {
        if (!Remove(meal.Id))
            throw new InvalidOperationException();

        Add(meal.Id, meal);
    }

    public void Remove(Meal meal)
    {
        if (!Remove(meal.Id))
            throw new InvalidOperationException();
    }
}
