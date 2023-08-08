using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mealmap.Infrastructure.DataAccess.EntityConfigurations;

public class MealEntityTypeConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("meal", MealmapDbContext.SCHEMA);

        builder.Property(m => m.Id);
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Version)
            .HasConversion(MealmapDbContext.Converters["VersionConverter"])
            .IsRowVersion();

        builder.Property(m => m.DiningDate)
            .HasColumnType("date")
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        builder.OwnsMany(m => m.Courses,
                course =>
                {
                    course.Property("Id");
                    course.HasKey("Id");
                    course.HasOne<Dish>().WithMany().HasForeignKey(c => c.DishId);
                    course.ToTable("course", MealmapDbContext.SCHEMA);
                });
    }
}
