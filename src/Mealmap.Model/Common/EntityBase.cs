namespace Mealmap.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; }

    public byte[]? Version { get; internal set; }

    public EntityBase()
        => Id = Guid.NewGuid();

    public EntityBase(Guid id)
        => Id = id;
}
