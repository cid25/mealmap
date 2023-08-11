using Mealmap.Domain;
using Mealmap.Domain.Common;
using Mealmap.Domain.Seedwork.Validation;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Infrastructure.DataAccess;

public class UnitOfWork : AbstractUnitOfWork
{
    private readonly MealmapDbContext _context;

    public UnitOfWork(IDeferredDomainValidator validator, MealmapDbContext context)
        : base(validator)
    {
        _context = context;
    }

    /// <exception cref="DomainValidationException"></exception>
    /// <exception cref="ConcurrentUpdateException"></exception>
    protected override async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrentUpdateException("An entity has been modified concurrently.");
        }
    }

    protected override IReadOnlyCollection<EntityBase> GetModifiedEntities()
    {
        return _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .OfType<EntityBase>()
            .ToList().AsReadOnly();
    }
}
