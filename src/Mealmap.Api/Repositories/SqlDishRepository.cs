using Mealmap.Model;

namespace Mealmap.Api.Repositories
{
    public class SqlDishRepository : IDishRepository
    {
        private MealmapDbContext _dbContext { get; }

        public SqlDishRepository(MealmapDbContext dbContext)
        { 
            _dbContext = dbContext;
        }

        public IEnumerable<Dish> GetAll()
        {
            var dishes = _dbContext.Dishes.ToList();
            
            return dishes;
        }

        public Dish? GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Create(Dish dish)
        {
            throw new NotImplementedException();
        }
    }
}
