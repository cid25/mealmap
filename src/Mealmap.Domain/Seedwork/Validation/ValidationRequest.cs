using Mealmap.Domain.Common;
using MediatR;

namespace Mealmap.Domain.Seedwork.Validation;

public class ValidationRequest<TEntity> : IRequest<DomainValidationResult>
    where TEntity : EntityBase
{
    public TEntity Entity { get; set; }

    public ValidationRequest(TEntity entity)
    {
        Entity = entity;
    }
}
