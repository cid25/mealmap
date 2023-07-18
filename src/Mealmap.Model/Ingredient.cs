using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Model
{
    [Owned]
    public record Ingredient
    {
        public decimal Quantity { get; init; }

        [NotMapped]
        public UnitOfMeasurement UnitOfMeasurement { get; init; }

        public string Description { get; init; }

        private UnitOfMeasurementCodes _unitOfMeasurementCode;

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
