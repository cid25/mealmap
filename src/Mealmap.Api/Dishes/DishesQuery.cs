using Mealmap.Api.Shared;
using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Dishes;

public class DishesQuery : QueryBase
{
    public string? Searchterm { get; set; }
    public int? Limit { get; init; }
    public Guid? Next { get; init; }

    public IQueryable<Dish> ComposeQuery(DbSet<Dish> db)
    {
        var query = db.AsNoTracking();

        if (Next != null) query = query.Where(d => d.Id >= Next);
        if (Searchterm != null) query = query.Where(d =>
            d.Name.Contains(Searchterm) || d.Description != null && d.Description.Contains(Searchterm)
        );

        return query.OrderBy(d => d.Id).Take(EffectiveLimit() + 1);
    }

    public int EffectiveLimit()
    {
        return Limit != null && Limit <= DEFAULTLIMIT ? (int)Limit : DEFAULTLIMIT;
    }
}
