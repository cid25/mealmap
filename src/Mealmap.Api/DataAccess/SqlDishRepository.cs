using Mealmap.Model;

namespace Mealmap.Api.DataAccess
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
            return _dbContext.Dishes.Find(id);
        }

        public void Create(Dish dish)
        {
            _dbContext.Dishes.Add(dish);
            _dbContext.SaveChanges();
        }
    }
}
