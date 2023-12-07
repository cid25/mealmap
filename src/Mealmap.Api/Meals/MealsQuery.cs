namespace Mealmap.Api.Meals;

public class MealsQuery(DateOnly? fromDate, DateOnly? toDate)
{
    public DateOnly? FromDate { get; init; } = fromDate;
    public DateOnly? ToDate { get; init; } = toDate;
}
