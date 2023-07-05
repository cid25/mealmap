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

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}
