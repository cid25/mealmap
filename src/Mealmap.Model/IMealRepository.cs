namespace Mealmap.Model
{
    public interface IMealRepository
    {
        public IEnumerable<Meal> GetAll();

        public Meal? GetById(Guid id);

        public void Create(Meal meal);
    }
}
