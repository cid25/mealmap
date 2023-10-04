using System.ComponentModel.DataAnnotations;
using Mealmap.Domain.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Domain.DishAggregate;

[Owned]
public class Ingredient
{
#pragma warning disable IDE0052
    private readonly Guid Id;

    [Precision(8, 2)]
    public decimal Quantity { get; internal init; }

    [MaxLength(30)]
    public string UnitOfMeasurement { get; internal init; }

    [MaxLength(100)]
    public string Description { get; internal init; }

#pragma warning restore IDE0052

    internal Ingredient(decimal quantity, string unitOfMeasurement, string description)
    {
        if (quantity <= 0)
            throw new DomainValidationException("The quantity of an ingredient must be larger than 0.");

        Id = Guid.NewGuid();

        Quantity = quantity;
        UnitOfMeasurement = unitOfMeasurement;
        Description = description;
    }
}
