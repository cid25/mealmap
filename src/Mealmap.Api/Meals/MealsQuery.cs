namespace Mealmap.Api.Meals;

public class MealsQuery
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }

    public MealsQuery(DateOnly? fromDate, DateOnly? toDate)
    {
        FromDate = fromDate;
        ToDate = toDate;
    }
}
