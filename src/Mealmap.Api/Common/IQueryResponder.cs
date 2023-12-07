namespace Mealmap.Api.Common;

public interface IQueryResponder<TQuery, TResponse>
{
    public Task<TResponse> RespondTo(TQuery query);
}
