namespace Mealmap.Domain.Common.Validation
{
    /// <summary>
    ///     Examines a collection of instances of <see cref="EntityBase"/> and throws a
    ///     <see cref="DomainValidationException"/> if any inconsistencies are found.
    /// </summary>
    /// <remarks>
    ///     The validation is deferred, i.e. it takes place after modifications have been done,
    ///     but bound to happen before persisting the changes or triggering
    ///     any side effects that are noticeable outside the application boundary.
    /// </remarks>
    public interface IDeferredDomainValidator
    {
        Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities);
    }
}
