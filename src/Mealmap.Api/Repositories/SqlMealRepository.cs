using Mealmap.Model;

namespace Mealmap.Api.Repositories
{
    public class SqlMealRepository : IMealRepository
    {
        private MealmapDbContext _dbContext { get; }

        public SqlMealRepository(MealmapDbContext dbContext)
        { 
            _dbContext = dbContext;
        }

        public IEnumerable<Meal> GetAll()
        {
            var meals = _dbContext.Meals.ToList();            
            
            return meals;
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
