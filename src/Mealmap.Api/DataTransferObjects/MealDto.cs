namespace Mealmap.Api.DataTransferObjects
{
    public record MealDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public MealDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
