using Mealmap.Domain;
using Mealmap.Domain.Common;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;
using Mealmap.Domain.UnitTests;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess;

public class DeferredDomainValidatorTests
{
    [Fact]
    public async void ValidateEntitiesAsync_WhenNoEntities_CompletesWithoutExceptionOrCallToServiceProvider()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();
        DeferredDomainValidator validator = new(serviceProvider.Object);
        var noEntities = new List<Meal>().AsReadOnly();

        // Act
        var act = async () => await validator.ValidateEntitiesAsync(noEntities);

        // Assert
        await act.Should().NotThrowAsync();
        serviceProvider.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ValidateEntitiesAsync_WhenEntityValid_CompletesWithoutException()
    {
        // Arrange
        DomainValidationResult validValidationResult = new();

        var validator = new DummyEntityValidator();

        var serviceProdiver = new Mock<IServiceProvider>();
        serviceProdiver.Setup(x => x.GetService(It.IsAny<Type>())).Returns(validator);

        DeferredDomainValidator deferredValidator = new(serviceProdiver.Object);

        // Act
        await deferredValidator.ValidateEntitiesAsync(new List<EntityBase>() {
            new DummyEntity(isValid: true) });
    }

    [Fact]
    public void ValidateEntitiesAsync_WhenEntityInvalid_ThrowsDomainValidationException()
    {
        // Arrange
        DomainValidationResult validValidationResult = new();

        var validator = new DummyEntityValidator();

        var serviceProdiver = new Mock<IServiceProvider>();
        serviceProdiver.Setup(x => x.GetService(It.IsAny<Type>())).Returns(validator);

        DeferredDomainValidator deferredValidator = new(serviceProdiver.Object);

        // Act
        var act = async () => await deferredValidator.ValidateEntitiesAsync(new List<EntityBase>() {
            new DummyEntity(isValid: false) });

        // Assert
        act.Should().ThrowAsync<DomainValidationException>();
    }
}
