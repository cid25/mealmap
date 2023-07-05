namespace Mealmap.Api.DataTransfer
{
    public record MealDTO
    {
        public Guid? Id { get; init; }

        public DateOnly Date { get; init; }

        public Guid? Dish { get; init; }
    }
}
