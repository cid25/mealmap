using Mealmap.Model;

namespace Mealmap.Repositories
{
    public class SqlMealRepository : IMealRepository
    {
        public IEnumerable<Meal> GetAll()
        {
            return new List<Meal>();
        }

        public Meal? GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Create(Meal meal)
        {
            throw new NotImplementedException();
        }
    }
}
