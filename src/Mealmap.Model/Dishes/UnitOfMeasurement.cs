using System.ComponentModel.DataAnnotations.Schema;

namespace Mealmap.Domain.DishAggregate;

[NotMapped]
public record UnitOfMeasurement
{
    private static readonly IEnumerable<UnitOfMeasurement> _units;

    public UnitOfMeasurementCodes UnitOfMeasurementCode { get; init; }

    public string Symbol { get; init; }

    public string? Measure { get; init; }

    static UnitOfMeasurement()
    {
        _units = new List<UnitOfMeasurement>()
            {
                new UnitOfMeasurement(UnitOfMeasurementCodes.Gram, "g", "Mass"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Kilogram, "kg", "Mass"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Liter, "l", "Volume"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Mililiter, "ml", "Volume"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Piece, "pcs", "Quantity"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Bag, "bag"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Can, "can"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Slice, "slc"),
                new UnitOfMeasurement(UnitOfMeasurementCodes.Pinch, "pnch"),
            };
    }

    private UnitOfMeasurement(UnitOfMeasurementCodes unitOfMeasurementCode, string symbol, string? measure = null)
        => (UnitOfMeasurementCode, Symbol, Measure) = (unitOfMeasurementCode, symbol, measure);

    public UnitOfMeasurement(UnitOfMeasurementCodes unitOfMeasurementCode)
    {
        var unit = _units.First(u => u.UnitOfMeasurementCode == unitOfMeasurementCode);
        (UnitOfMeasurementCode, Symbol, Measure) = (unitOfMeasurementCode, unit.Symbol, unit.Measure);
    }

    public UnitOfMeasurement(string unitOfMeasurement)
    {
        try
        {
            var code = (UnitOfMeasurementCodes)Enum.Parse(typeof(UnitOfMeasurementCodes), unitOfMeasurement);
            var unit = _units.First(u => u.UnitOfMeasurementCode == code);

            UnitOfMeasurementCode = code;
            Symbol = unit.Symbol;
            Measure = unit.Measure;

        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"UnitOfMeasurement must match an existing unit of measurement, but is {0}.", unitOfMeasurement);
        }

    }

    public string Stringify()
    {
        return Enum.GetName(typeof(UnitOfMeasurementCodes), UnitOfMeasurementCode)!;
    }
}
