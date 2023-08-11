using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Domain.DishAggregate;

[Owned]
public class Ingredient : IEquatable<Ingredient>
{
#pragma warning disable IDE0052
    private readonly Guid Id;

    [Precision(8, 2)]
    public decimal Quantity { get; internal init; }

    [NotMapped]
    public UnitOfMeasurement UnitOfMeasurement { get; internal init; }

    public string Description { get; internal init; }


    private readonly UnitOfMeasurementCodes _unitOfMeasurementCode;
#pragma warning restore IDE0052

    internal Ingredient(decimal quantity, UnitOfMeasurementCodes unitOfMeasurementCode, string description)
    {
        Id = Guid.NewGuid();

        Quantity = quantity;
        _unitOfMeasurementCode = unitOfMeasurementCode;
        UnitOfMeasurement = new UnitOfMeasurement(unitOfMeasurementCode);
        Description = description;
    }

    internal Ingredient(decimal quantity, UnitOfMeasurement unitOfMeasurement, string description)
        : this(quantity, unitOfMeasurement.UnitOfMeasurementCode, description) { }

    internal Ingredient(decimal quantity, string unitOfMeasurement, string description)
        : this(quantity, new UnitOfMeasurement(unitOfMeasurement), description) { }

    public bool Equals(Ingredient? other)
    {
        if (other is null)
            return false;
        return (Quantity, UnitOfMeasurement.UnitOfMeasurementCode, Description)
            == (other.Quantity, other.UnitOfMeasurement.UnitOfMeasurementCode, other.Description);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Ingredient);
    }

    public override int GetHashCode()
    {
        return (Quantity, UnitOfMeasurement.UnitOfMeasurementCode, Description).GetHashCode();
    }
}
