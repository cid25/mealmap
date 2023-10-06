using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Queries;

public class DishQuery : QueryBase, IRequest<PaginatedDTO<DishDTO>>
{
    public string? Searchterm { get; set; }
    public int? Limit { get; init; }
    public Guid? Next { get; init; }

    public IQueryable<Dish> Query(DbSet<Dish> db)
    {
        var query = db.AsNoTracking();

        if (Next != null) query = query.Where(d => d.Id >= Next);

        return query.OrderBy(d => d.Id).Take(EffectiveLimit() + 1);
    }

    public int EffectiveLimit()
    {
        return Limit != null && Limit <= DEFAULTLIMIT ? (int)Limit : DEFAULTLIMIT;
    }
}
