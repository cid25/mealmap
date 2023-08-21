using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.Common.DataAccess;

/// <inheritdoc cref="IUnitOfWork" />
public abstract class AbstractUnitOfWork : IUnitOfWork
{
    private readonly IDeferredDomainValidator _validator;

    public AbstractUnitOfWork(IDeferredDomainValidator validator)
    {
        _validator = validator;
    }

    /// <inheritdoc cref="IUnitOfWork.SaveTransactionAsync" />
    public async Task SaveTransactionAsync()
    {
        await _validator.ValidateEntitiesAsync(GetModifiedEntities());
        await SaveChangesAsync();
    }

    /// <summary>
    ///     Persists the changes done to all instances of <see cref="EntityBase"/> and related objects that are
    ///     tracked by this Unit of Work.
    /// </summary>
    /// <remarks>
    ///     Throws a <see cref="ConcurrentUpdateException"/> in case of optimistic concurrency errors during updates,
    ///     and an <see cref="UpdateException"/> if any other error occurs when trying to persist the changes of this Unit of Work.
    /// </remarks>
    /// <exception cref="ConcurrentUpdateException" />
    /// <exception cref="UpdateException" />
    protected abstract Task SaveChangesAsync();

    /// <summary>
    ///     Returns the set of instances of <see cref="EntityBase"/> that have been modified (i.e. created, deleted, or changed)
    ///     during the lifetime of this Unit of Work.
    /// </summary>
    /// <remarks>
    ///     Is called in <see cref="SaveTransactionAsync" /> to provide the entities that are to be validated by the
    ///     <see cref="IDeferredDomainValidator"/> before persisting the model.
    /// </remarks>
    protected abstract IReadOnlyCollection<EntityBase> GetModifiedEntities();
}
