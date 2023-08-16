namespace Mealmap.Domain.Common.Validation
{
    public interface IDeferredDomainValidator
    {
        Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities);
    }
}
