using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.DataAccess;

internal static class ModelBuilderExtensions
{
    private const string Schema = "mealmap";

    internal static void ConfigureDish(this ModelBuilder modelBuilder)
    {
        var dish = modelBuilder.Entity<Dish>();

        dish.ToTable("dish", Schema);

        dish.OwnsMany(d => d.Ingredients, i =>
            {
                i.Property("_unitOfMeasurementCode").HasColumnName("UnitOfMeasurementCode");
                i.ToTable("ingredient", Schema);
            });
    }

    internal static void ConfigureMeal(this ModelBuilder modelBuilder)
    {
        var meal = modelBuilder.Entity<Meal>();

        meal.ToTable("meal", Schema);

        meal.Property(m => m.Id);
        meal.HasKey(m => m.Id);

        meal.Property(m => m.DiningDate)
            .HasColumnType("date")
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        meal.OwnsMany(meal => meal.Courses,
                course =>
                {
                    course.HasOne<Dish>().WithMany().HasForeignKey(course => course.DishId);
                    course.ToTable("course", Schema);
                });
    }
}
