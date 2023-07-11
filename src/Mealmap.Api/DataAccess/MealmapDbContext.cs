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
                .Property(e => e.Date)
                .HasColumnType("date")
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v)
                );
        }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}
