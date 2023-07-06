using Mealmap.Model;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Repositories
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
                    v => v.ToString("yyyy-MM-dd"),
                    v => DateOnly.Parse(v)
                );
        }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}
