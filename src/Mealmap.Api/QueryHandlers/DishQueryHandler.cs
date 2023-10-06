using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.Queries;
using Mealmap.Domain.DishAggregate;
using Mealmap.Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.QueryHandlers;

public class DishQueryHandler : IRequestHandler<DishQuery, PaginatedDTO<DishDTO>>
{
    private readonly MealmapDbContext _db;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly UrlBuilder _urlBuilder;

    public DishQueryHandler(MealmapDbContext db, IOutputMapper<DishDTO, Dish> outputMapper, UrlBuilder urlBuilder)
    {
        _db = db;
        _outputMapper = outputMapper;
        _urlBuilder = urlBuilder;
    }

    public async Task<PaginatedDTO<DishDTO>> Handle(DishQuery request, CancellationToken cancellationToken)
    {
        PaginatedDTO<DishDTO> result = new();

        var query = request.Query(_db.Set<Dish>());

        var queryResult = await query.ToArrayAsync(cancellationToken);

        if (queryResult.Length > request.EffectiveLimit())
        {
            result.Items = _outputMapper.FromEntities(queryResult.Take(request.EffectiveLimit()));
            result.Next = _urlBuilder.NextPage("/api/dishes", queryResult.Last().Id, request.EffectiveLimit());
        }
        else result.Items = _outputMapper.FromEntities(queryResult);

        return result;
    }
}
