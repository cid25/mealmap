using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.Common.DataAccess;

public abstract class AbstractUnitOfWork : IUnitOfWork
{
    private readonly IDeferredDomainValidator _validator;

    public AbstractUnitOfWork(IDeferredDomainValidator validator)
    {
        _validator = validator;
    }

    /// <exception cref="DomainValidationException"></exception>
    /// <exception cref="ConcurrentUpdateException"></exception>
    public async Task SaveTransactionAsync()
    {
        await _validator.ValidateEntitiesAsync(GetModifiedEntities());
        await SaveChangesAsync();
    }

    /// <exception cref="ConcurrentUpdateException"></exception>
    protected abstract Task SaveChangesAsync();

    protected abstract IReadOnlyCollection<EntityBase> GetModifiedEntities();
}
