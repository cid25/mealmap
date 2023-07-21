using Mealmap.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.BroadIntegrationTests
{
    internal class Helpers
    {
        public static void DetachAllEntities(MealmapDbContext dbContext)
        {
            var undetachedEntriesCopy = dbContext.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
