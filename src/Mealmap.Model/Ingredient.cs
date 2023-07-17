using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Model
{
    [Owned]
    public record Ingredient
    {
        public decimal Quantity { get; init; }

        public UnitOfMeasurement UnitOfMeasurement { get; init; }

        public string Description { get; init; }

        public Ingredient(decimal quantity, string unitOfMeasurementName, string description)
        {
            Quantity = quantity;
            UnitOfMeasurement = UnitOfMeasurement.FromName(unitOfMeasurementName);
            Description = description;
        }
    }
}
