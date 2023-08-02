using System.ComponentModel.DataAnnotations;

namespace Mealmap.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; }

    [Timestamp]
    public byte[]? Version { get; set; }

    public EntityBase()
        => Id = Guid.NewGuid();

    public EntityBase(Guid id)
        => Id = id;
}
