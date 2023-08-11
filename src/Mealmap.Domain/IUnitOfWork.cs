namespace Mealmap.Domain
{
    public interface IUnitOfWork
    {
        Task SaveTransactionAsync();
    }
}
