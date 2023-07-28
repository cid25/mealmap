using Mealmap.Domain.Exceptions;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.DataAccess;

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

    /// <exception cref="DomainValidationException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
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
                throw new DomainValidationException("A given dish does not exist.");
            else
                throw;
        }
    }

    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    public void Remove(Meal meal)
    {
        var removable = _dbContext.Meals.Find(meal.Id);

        if (removable != null)
        {
            _dbContext.Meals.Remove(removable);
            _dbContext.SaveChanges();
        }
    }
}
