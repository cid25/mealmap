using Mealmap.Model;
using FluentAssertions;

namespace Mealmap.Model.Tests
{
    public class UnitOfMeasurementTests
    {
        [Fact]
        public void FromName_WhenValidName_ReturnsUnitOfMeasurement()
        {
            const string validUnitName = "Kilogram";

            var result = UnitOfMeasurement.FromName(validUnitName);

            result.Should().NotBeNull();
            result.Should().BeOfType<UnitOfMeasurement>();
        }

        [Fact]
        public void FromName_WhenInvalidName_ReturnsUnitOfMeasurement()
        {
            const string invalidUnitName = "Kawabunga";

            Action action = () => UnitOfMeasurement.FromName(invalidUnitName);

            action.Should().Throw<ArgumentException>();
        }
    }
}
