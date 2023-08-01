using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

public class MealmapDbContext : DbContext
{
    public MealmapDbContext(DbContextOptions<MealmapDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureDish();

        modelBuilder.ConfigureMeal();
    }

    public DbSet<Meal> Meals { get; set; }
    public DbSet<Dish> Dishes { get; set; }
}
