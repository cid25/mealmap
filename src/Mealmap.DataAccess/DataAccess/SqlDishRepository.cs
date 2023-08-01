using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

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

    public Dish? GetSingleById(Guid id)
    {
        return _dbContext.Dishes.Find(id);
    }

    public void Add(Dish dish)
    {
        _dbContext.Dishes.Add(dish);
        _dbContext.SaveChanges();
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ConcurrentUpdateException"></exception>
    public void Update(Dish dish)
    {
        var existingDish = _dbContext.Dishes.Find(dish.Id);
        if (existingDish == null || existingDish != dish)
            throw new InvalidOperationException();

        try
        {
            MarkIngredientsForReplacement();
            MarkDishForVersionUpdate(dish);
            AdoptVersionFromClient(dish);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentUpdateException("Saving dish failed due to version mismatch.", ex);
        }
    }

    public void Remove(Dish dish)
    {
        _dbContext.Remove(dish);
        _dbContext.SaveChanges();
    }

    private void MarkIngredientsForReplacement()
    {
        foreach (var ingredient in _dbContext.ChangeTracker.Entries().
           Where(e => e.Entity is Ingredient && e.State == EntityState.Modified))
            ingredient.State = EntityState.Added;
    }

    private void MarkDishForVersionUpdate(Dish dish)
    {
        _dbContext.Entry(dish).Property(d => d.Name).IsModified = true;
    }

    private void AdoptVersionFromClient(Dish dish)
    {
        _dbContext.Entry(dish).OriginalValues[nameof(Dish.Version)] = dish.Version;
    }
}
