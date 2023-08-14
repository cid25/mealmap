using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess.Repositories;

public class SqlDishRepository : IDishRepository
{
    private MealmapDbContext _dbContext { get; }
    internal DbSet<Dish> dbSet { get; }

    public SqlDishRepository(MealmapDbContext dbContext)
    {
        _dbContext = dbContext;
        dbSet = dbContext.Set<Dish>();
    }

    public IEnumerable<Dish> GetAll()
    {
        var dishes = dbSet.ToList();

        return dishes;
    }

    public Dish? GetSingleById(Guid id)
    {
        return dbSet.Find(id);
    }

    public void Add(Dish dish)
    {
        dbSet.Add(dish);
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ConcurrentUpdateException"></exception>
    public void Update(Dish dish)
    {
        var existingDish = dbSet.Find(dish.Id);
        if (existingDish == null || existingDish != dish)
            throw new InvalidOperationException();

        MarkIngredientsForReplacement();
        MarkDishForVersionUpdate(dish);
        AdoptVersionFromClient(dish);
    }

    public void Remove(Dish dish)
    {
        _dbContext.Remove(dish);
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
