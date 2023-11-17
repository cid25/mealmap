using Mealmap.Api.Shared;
using Mealmap.Domain.DishAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Dishes;

public class DishesQueryResponder(MealmapDbContext db, IOutputMapper<DishDTO, Dish> outputMapper, UrlBuilder urlBuilder) : IQueryResponder<DishesQuery, PaginatedDTO<DishDTO>>
{
    public async Task<PaginatedDTO<DishDTO>> RespondTo(DishesQuery query)
    {
        PaginatedDTO<DishDTO> result = new();

        var composedQuery = query.ComposeQuery(db.Set<Dish>());

        var queryResult = await composedQuery.ToArrayAsync();

        if (queryResult.Length > query.EffectiveLimit())
        {
            result.Items = outputMapper.FromEntities(queryResult.Take(query.EffectiveLimit()));
            result.Next = urlBuilder.NextPage("/api/dishes", queryResult.Last().Id, query.EffectiveLimit());
        }
        else result.Items = outputMapper.FromEntities(queryResult);

        return result;
    }
}
