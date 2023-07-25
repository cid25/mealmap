using Mealmap.Model;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.DataAccess
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

        public Dish? GetSingle(Guid id)
        {
            return _dbContext.Dishes.Find(id);
        }

        public void Add(Dish dish)
        {
            _dbContext.Dishes.Add(dish);
            _dbContext.SaveChanges();
        }

        public void Update(Dish dish)
        {
            var existingDish = _dbContext.Dishes.Find(dish.Id);
            if (existingDish == null)
                return;

            RemoveIngredientsFrom(existingDish);
            _dbContext.Remove(existingDish);

            _dbContext.Update(dish);
            if (existingDish.Image != null)
                dish.Image = existingDish.Image with { };
            AddIngredientsTo(dish);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
        }

        public void Remove(Dish dish)
        {
            _dbContext.Remove(dish);
            _dbContext.SaveChanges();
        }

        private void RemoveIngredientsFrom(Dish dish)
        {
            if (dish.Ingredients != null)
                _dbContext.RemoveRange(dish.Ingredients);
        }

        private void AddIngredientsTo(Dish dish)
        {
            if (dish.Ingredients != null)
                _dbContext.AddRange(dish.Ingredients);
        }
    }
}
