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

        public void Update(Dish dish, bool retainImage = true)
        {
            var existingDish = _dbContext.Dishes.Find(dish.Id);
            if (existingDish == null)
                return;

            UnregisterIngredients(existingDish);
            _dbContext.Remove(existingDish);

            _dbContext.Update(dish);
            RegisterIngredients(dish);
            ResolveImage(dish, existingDish, retainImage);

            if (dish.Image != existingDish.Image)
            {
                if (existingDish.Image != null && retainImage)
                    dish.Image = existingDish.Image with { };
                if (dish.Image != null && !retainImage)
                    dish.Image = dish.Image with { };
            }

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

        private void UnregisterIngredients(Dish dish)
        {
            if (dish.Ingredients != null)
                _dbContext.RemoveRange(dish.Ingredients);
        }

        private void RegisterIngredients(Dish dish)
        {
            if (dish.Ingredients != null)
                _dbContext.AddRange(dish.Ingredients);
        }

        private static void ResolveImage(Dish newDish, Dish oldDish, bool retainImage)
        {
            if (retainImage && oldDish.Image != null)
                newDish.Image = oldDish.Image with { };
            else if (!retainImage && newDish.Image != oldDish.Image)
            {
                if (newDish.Image != null)
                    newDish.Image = newDish.Image with { };
                else
                    newDish.Image = null;
            }
        }
    }
}
