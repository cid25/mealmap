using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

internal static class ModelBuilderExtensions
{
    private const string Schema = "mealmap";

    internal static void ConfigureDish(this ModelBuilder modelBuilder)
    {
        var dish = modelBuilder.Entity<Dish>();

        dish.ToTable("dish", Schema);

        dish.Property(d => d.Id);
        dish.HasKey(d => d.Id);

        dish.Property(d => d.Name);
        dish.Property(d => d.Description);
        dish.Property(d => d.Servings);

        dish.OwnsOne(dish => dish.Image, image =>
        {
            image.Property(i => i.Content);
            image.Property(i => i.ContentType);
        });

        dish.OwnsMany(d => d.Ingredients, i =>
            {
                i.ToTable("ingredient", Schema);
                i.Property("Id");
                i.HasKey("Id");
                i.Property("_unitOfMeasurementCode").HasColumnName("UnitOfMeasurementCode");
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
                    course.Property("Id");
                    course.HasKey("Id");
                    course.HasOne<Dish>().WithMany().HasForeignKey(c => c.DishId);
                    course.ToTable("course", Schema);
                });
    }
}
