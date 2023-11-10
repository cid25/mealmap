using Mealmap.Api.Shared;
using Mealmap.Domain.DishAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Dishes;

public class DishesQueryResponder : IQueryResponder<DishesQuery, PaginatedDTO<DishDTO>>
{
    private readonly MealmapDbContext _db;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly UrlBuilder _urlBuilder;

    public DishesQueryResponder(MealmapDbContext db, IOutputMapper<DishDTO, Dish> outputMapper, UrlBuilder urlBuilder)
    {
        _db = db;
        _outputMapper = outputMapper;
        _urlBuilder = urlBuilder;
    }

    public async Task<PaginatedDTO<DishDTO>> RespondTo(DishesQuery query)
    {
        PaginatedDTO<DishDTO> result = new();

        var composedQuery = query.ComposeQuery(_db.Set<Dish>());

        var queryResult = await composedQuery.ToArrayAsync();

        if (queryResult.Length > query.EffectiveLimit())
        {
            result.Items = _outputMapper.FromEntities(queryResult.Take(query.EffectiveLimit()));
            result.Next = _urlBuilder.NextPage("/api/dishes", queryResult.Last().Id, query.EffectiveLimit());
        }
        else result.Items = _outputMapper.FromEntities(queryResult);

        return result;
    }
}
