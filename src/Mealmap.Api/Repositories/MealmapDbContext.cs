using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Repositories
{
    public class MealmapDbContext : DbContext
    {
        public MealmapDbContext(DbContextOptions<MealmapDbContext> options)
            : base(options)
        {
        }
    }
}
