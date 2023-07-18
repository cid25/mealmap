using Mealmap.Model;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.DataAccess
{
    public class MealmapDbContext : DbContext
    {
        public MealmapDbContext(DbContextOptions<MealmapDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Meal>()
                .Property(m => m.DiningDate)
                .HasColumnType("date")
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v)
                );
            modelBuilder
                .Entity<Meal>()
                .Navigation(m => m.Dish)
                .AutoInclude();


            modelBuilder
                .Entity<Dish>()
                .OwnsMany(d => d.Ingredients, i =>
                {
                    i.Property("_unitOfMeasurementCode").HasColumnName("UnitOfMeasurementCode");
                });
        }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}
