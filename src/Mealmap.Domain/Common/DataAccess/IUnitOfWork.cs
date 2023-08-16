using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.Common.DataAccess;

public interface IUnitOfWork
{
    /// <exception cref="DomainValidationException"></exception>
    Task SaveTransactionAsync();
}
