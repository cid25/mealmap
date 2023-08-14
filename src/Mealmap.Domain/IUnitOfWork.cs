using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Domain;

public interface IUnitOfWork
{
    /// <exception cref="DomainValidationException"></exception>
    Task SaveTransactionAsync();
}
