namespace Mealmap.Domain.MealAggregate;

public interface IMealRepository
{
    public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null);

    public Meal? GetSingleById(Guid id);

    public void Add(Meal meal);

    public void Remove(Meal meal);
}
