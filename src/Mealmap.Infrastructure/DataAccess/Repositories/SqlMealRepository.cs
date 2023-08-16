using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess.Repositories;

public class SqlMealRepository : IMealRepository
{
    private MealmapDbContext _dbContext { get; }
    internal DbSet<Meal> dbSet { get; }

    public SqlMealRepository(MealmapDbContext dbContext)
    {
        _dbContext = dbContext;
        dbSet = dbContext.Set<Meal>();
    }

    public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var meals = dbSet.AsQueryable();

        if (fromDate != null)
            meals = meals.Where(m => m.DiningDate >= fromDate);
        if (toDate != null)
            meals = meals.Where(m => m.DiningDate <= toDate);

        return meals.ToList();
    }

    public Meal? GetSingleById(Guid id)
    {
        var meal = dbSet.FirstOrDefault(x => x.Id == id);

        return meal;
    }

    /// <exception cref="ConcurrentUpdateException"></exception>
    public void Add(Meal meal)
    {
        dbSet.Add(meal);
    }

    /// <exception cref="InvalidOperationException"></exception>

    public void Update(Meal meal)
    {
        var existingMeal = dbSet.Find(meal.Id);
        if (existingMeal == null || existingMeal != meal)
            throw new InvalidOperationException();

        MarkCoursesForReplacement();
        MarkMealForVersionUpdate(meal);
        AdoptVersionFromClient(meal);
    }

    public void Remove(Meal meal)
    {
        var removable = dbSet.Find(meal.Id);

        if (removable != null)
        {
            dbSet.Remove(removable);
        }
    }

    private void MarkCoursesForReplacement()
    {
        foreach (var course in _dbContext.ChangeTracker.Entries().
           Where(e => e.Entity is Course && e.State == EntityState.Modified))
            course.State = EntityState.Added;
    }

    private void MarkMealForVersionUpdate(Meal meal)
    {
        _dbContext.Entry(meal).Property(d => d.DiningDate).IsModified = true;
    }

    private void AdoptVersionFromClient(Meal meal)
    {
        _dbContext.Entry(meal).OriginalValues[nameof(Meal.Version)] = meal.Version;
    }
}
