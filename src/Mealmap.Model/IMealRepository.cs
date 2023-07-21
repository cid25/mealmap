namespace Mealmap.Model
{
    public interface IMealRepository
    {
        public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null);

        public Meal? GetSingle(Guid id);

        public void Add(Meal meal);

        public void Remove(Meal meal);
    }
}
