using FluentAssertions;

namespace Mealmap.Model.Tests
{
    public class UnitOfMeasurementTests
    {
        [Fact]
        public void From_WhenValidName_ReturnsUnitOfMeasurement()
        {
            const string validUnitName = "Kilogram";

            var result = new UnitOfMeasurement(validUnitName);

            result.Should().NotBeNull();
            result.Should().BeOfType<UnitOfMeasurement>();
        }

        [Fact]
        public void From_WhenInvalidName_ThrowsArgumentException()
        {
            const string invalidUnitName = "Kawabunga";

            Action action = () =>
            {
                var unitOfMeasurement = new UnitOfMeasurement(invalidUnitName);
            };

            action.Should().Throw<ArgumentException>();
        }
    }
}
