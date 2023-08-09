namespace Mealmap.Domain.Common;

public abstract class AbstractUnitOfWork
{
    public void SaveTransaction()
    {
        // Domain Validation here
        // ...

        SaveChanges();
    }

    abstract protected void SaveChanges();
}
