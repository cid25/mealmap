using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

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

    public Meal? GetSingleById(Guid id)
    {
        var meal = _dbContext.Meals.FirstOrDefault(x => x.Id == id);

        return meal;
    }

    /// <exception cref="ConcurrentUpdateException"></exception>
    public void Add(Meal meal)
    {

        _dbContext.Meals.Add(meal);
        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("DishId"))
                throw new ConcurrentUpdateException("A given dish does not exist.", ex);
            else
                throw;
        }
    }

    public void Update(Meal meal)
    {
        var existingMeal = _dbContext.Meals.Find(meal.Id);
        if (existingMeal == null || existingMeal != meal)
            throw new InvalidOperationException();

        try
        {
            MarkCoursesForReplacement();
            MarkMealForVersionUpdate(meal);
            AdoptVersionFromClient(meal);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentUpdateException("Saving meal failed due to version mismatch.", ex);
        }
    }

    public void Remove(Meal meal)
    {
        var removable = _dbContext.Meals.Find(meal.Id);

        if (removable != null)
        {
            _dbContext.Meals.Remove(removable);
            _dbContext.SaveChanges();
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
