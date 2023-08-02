namespace Mealmap.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; }

    public EntityVersion Version { get; set; }

    public EntityBase()
    {
        Id = Guid.NewGuid();
        Version = new EntityVersion(String.Empty);
    }

    public EntityBase(Guid id)
    {
        Id = id;
        Version = new EntityVersion(String.Empty);
    }
}
