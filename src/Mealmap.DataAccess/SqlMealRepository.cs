using Mealmap.Model;

namespace Mealmap.DataAccess
{
    public class SqlMealRepository : IMealRepository
    {
        private MealmapDbContext _dbContext { get; }

        public SqlMealRepository(MealmapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var meals = _dbContext.Meals.AsQueryable();

            if (fromDate != null)
                meals = meals.Where(m => m.DiningDate >= fromDate);
            if (toDate != null)
                meals = meals.Where(m => m.DiningDate <= toDate);

            return meals.ToList();
        }

        public Meal? GetSingle(Guid id)
        {
            var meal = _dbContext.Meals.FirstOrDefault(x => x.Id == id);

            return meal;
        }

        public void Add(Meal meal)
        {
            _dbContext.Meals.Add(meal);
            _dbContext.SaveChanges();
        }

        public void Remove(Meal meal)
        {
            _dbContext.Meals.Remove(meal);
            _dbContext.SaveChanges();
        }
    }
}
