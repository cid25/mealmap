using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mealmap.Infrastructure.DataAccess;

public class MealmapDbContext(DbContextOptions<MealmapDbContext> options) : DbContext(options)
{
    public const string SCHEMA = "mealmap";

    private static readonly Dictionary<string, ValueConverter> _converters;

    public static IReadOnlyDictionary<string, ValueConverter> Converters { get => _converters.AsReadOnly(); }

    static MealmapDbContext()
    {
        _converters = new()
        {
            {
                "VersionConverter",
                new ValueConverter<EntityVersion, byte[]>(
                    v => v.AsBytes(),
                    v => new EntityVersion(v))
            }
        };
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new DishEntityTypeConfiguration().Configure(modelBuilder.Entity<Dish>());

        new MealEntityTypeConfiguration().Configure(modelBuilder.Entity<Meal>());
    }
}
