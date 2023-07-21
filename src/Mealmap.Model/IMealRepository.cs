namespace Mealmap.Model
{
    public interface IMealRepository
    {
        public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null);

        public Meal? GetById(Guid id);

        public void Create(Meal meal);
    }
}
