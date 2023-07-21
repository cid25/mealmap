using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Model
{
    [Owned]
    [Table("ingredients", Schema = "mealmap")]
    public record Ingredient
    {
        [Precision(8, 2)]
        public decimal Quantity { get; init; }

        [NotMapped]
        public UnitOfMeasurement UnitOfMeasurement { get; init; }

        public string Description { get; init; }

# pragma warning disable IDE0052
        private readonly UnitOfMeasurementCodes _unitOfMeasurementCode;
# pragma warning restore IDE0052

        public Ingredient(decimal quantity, UnitOfMeasurement unitOfMeasurement, string description)
        {
            Quantity = quantity;
            _unitOfMeasurementCode = unitOfMeasurement.UnitOfMeasurementCode;
            UnitOfMeasurement = unitOfMeasurement;
            Description = description;
        }

        public Ingredient(decimal quantity, UnitOfMeasurementCodes unitOfMeasurementCode, string description)
        {
            Quantity = quantity;
            _unitOfMeasurementCode = unitOfMeasurementCode;
            UnitOfMeasurement = new UnitOfMeasurement(unitOfMeasurementCode);
            Description = description;
        }
    }
}
