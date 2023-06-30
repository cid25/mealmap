namespace Mealmap.Model
{
    public interface IMealRepository
    {
        public Meal? GetById(Guid id);

        public void Create(Meal meal);
    }
}
