using Mealmap.Domain.Common;
using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Domain;

public abstract class AbstractUnitOfWork : IUnitOfWork
{
    private readonly IDeferredDomainValidator _validator;

    public AbstractUnitOfWork(IDeferredDomainValidator validator)
    {
        _validator = validator;
    }

    /// <exception cref="DomainValidationException"></exception>
    public async Task SaveTransactionAsync()
    {
        await _validator.ValidateEntitiesAsync(GetModifiedEntities());
        await SaveChangesAsync();
    }

    /// <exception cref="DomainValidationException"></exception>
    protected abstract Task SaveChangesAsync();

    protected abstract IReadOnlyCollection<EntityBase> GetModifiedEntities();
}
