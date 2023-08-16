namespace Mealmap.Domain.Common.DataAccess;

public interface IRepository<T>
{
    public T? GetSingleById(Guid id);

    public void Add(T entity);

    public void Update(T entity);

    public void Remove(T entity);
}
