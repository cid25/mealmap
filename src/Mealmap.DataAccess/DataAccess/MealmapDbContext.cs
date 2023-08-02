using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mealmap.Infrastructure.DataAccess;

public class MealmapDbContext : DbContext
{
    public MealmapDbContext(DbContextOptions<MealmapDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Dictionary<string, ValueConverter> converters = new()
        {
            {
                "VersionConverter",
                new ValueConverter<EntityVersion, byte[]>(
                    v => v.AsBytes(),
                    v => new EntityVersion(v))
            }
        };

        modelBuilder.ConfigureDish(converters);

        modelBuilder.ConfigureMeal(converters);
    }

    public DbSet<Meal> Meals { get; set; }
    public DbSet<Dish> Dishes { get; set; }
}
