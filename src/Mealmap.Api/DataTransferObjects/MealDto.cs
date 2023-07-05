using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransferObjects
{
    public record MealDTO
    {
        public Guid? Id { get; init; }
    }
}
