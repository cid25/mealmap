using Mealmap.Domain.Common;

namespace Mealmap.Domain
{
    public interface IDeferredDomainValidator
    {
        Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities);
    }
}