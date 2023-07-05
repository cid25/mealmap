using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransferObjects
{
    public record DishDTO
    {
        public Guid? Id { get; init; }

        [Required]
        public string Name { get; init; }

        public DishDTO(string name)
        {
            Name = name;
        }
    }
}
