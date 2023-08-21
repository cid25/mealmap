using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.Common.DataAccess;

/// <summary>
///     Represents the changes done to the model during its lifetime and the means to persist those.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Persists the changes tracked by this Unit of Work. 
    ///     Runs the configured <see cref="IDeferredDomainValidator"/> prior to that to detect any inconsistencies in the model.
    /// </summary>
    /// <exception cref="DomainValidationException" />
    /// <exception cref="ConcurrentUpdateException" />
    /// <exception cref="UpdateException" />
    Task SaveTransactionAsync();
}
