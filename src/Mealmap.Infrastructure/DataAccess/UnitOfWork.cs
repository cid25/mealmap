using Mealmap.Domain.Common;

namespace Mealmap.Infrastructure.DataAccess;

public class UnitOfWork : AbstractUnitOfWork
{
    private readonly MealmapDbContext _context;

    public UnitOfWork(MealmapDbContext context)
    {
        _context = context;
    }

    protected override sealed void SaveChanges()
    {
        _context.SaveChanges();
    }
}
