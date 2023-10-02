using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mealmap.Infrastructure.DataAccess.EntityConfigurations;

public class DishEntityTypeConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("dish", MealmapDbContext.SCHEMA);

        builder.Property(d => d.Id);
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Version)
            .HasConversion(MealmapDbContext.Converters["VersionConverter"])
            .IsRowVersion();

        builder.Property(d => d.Name);
        builder.Property(d => d.Description);
        builder.Property(d => d.Servings);
        builder.Property(d => d.Instructions);

        builder.OwnsOne(d => d.Image, image =>
        {
            image.Property(i => i.Content);
            image.Property(i => i.ContentType);
        });

        builder.OwnsMany(d => d.Ingredients, i =>
        {
            i.ToTable("ingredient", MealmapDbContext.SCHEMA);
            i.Property("Id");
            i.HasKey("Id");
            i.Property("_unitOfMeasurementCode").HasColumnName("UnitOfMeasurementCode");
        });
    }
}
