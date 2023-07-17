using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mealmap.Model
{
    [NotMapped]
    public class UnitOfMeasurement
    {
        private static readonly IEnumerable<UnitOfMeasurement> _units;

        public string Name { get; init; }

        public string Symbol { get; init; }

        public string? Measure { get; init; }

        static UnitOfMeasurement()
        {
            _units = new List<UnitOfMeasurement>()
            {
                new UnitOfMeasurement("Gram", "g", "Mass"),
                new UnitOfMeasurement("Kilogram", "kg", "Mass"),
                new UnitOfMeasurement("Liter", "l", "Volume"),
                new UnitOfMeasurement("Mililiter", "ml", "Volume"),
                new UnitOfMeasurement("Piece", "pc", "Quantity"),
                new UnitOfMeasurement("Bag", ""),
                new UnitOfMeasurement("Can", ""),
            };
        }

        private UnitOfMeasurement(string name, string symbol, string? measure = null)
            => (Name, Symbol, Measure) = (name, symbol, measure);

        public static UnitOfMeasurement FromName(string name)
        {
            try
            {
                return _units.First(x => x.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("{name} must match an existing unit of measurement", name);
            }
        }
    }
}
