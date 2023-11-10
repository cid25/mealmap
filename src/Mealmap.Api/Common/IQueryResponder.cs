namespace Mealmap.Api.Shared;

public interface IQueryResponder<TQuery, TResponse>
{
    public Task<TResponse> RespondTo(TQuery query);
}
