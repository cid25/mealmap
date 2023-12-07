using Mealmap.Domain.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

public class UnitOfWork(IDeferredDomainValidator validator, MealmapDbContext context) : AbstractUnitOfWork(validator)
{
    /// <exception cref="ConcurrentUpdateException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    protected override async Task SaveChangesAsync()
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentUpdateException("An entity has been modified concurrently.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UpdateException("An issue has occured when trying to persist changes.", ex);
        }
    }

    protected override IReadOnlyCollection<EntityBase> GetModifiedEntities()
    {
        return context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .OfType<EntityBase>()
            .ToList().AsReadOnly();
    }
}
