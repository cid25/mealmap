using Mealmap.Domain.Common;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Domain.UnitTests;

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

    [Fact]
    public async void ValidateEntitiesAsync_CallsForCorrectValidatorOnly()
    {
        // Arrange
        DomainValidationResult validValidationResult = new();

        var applicableValidator = new DummyEntityValidator();

        var serviceProdiver = new Mock<IServiceProvider>();
        serviceProdiver.Setup(x => x.GetService(It.IsAny<Type>())).Returns(applicableValidator);

        DeferredDomainValidator deferredValidator = new(serviceProdiver.Object);

        // Act
        await deferredValidator.ValidateEntitiesAsync(new List<EntityBase>() {
            new DummyEntity(isValid: true) });

        // Assert
        serviceProdiver.Verify(x => x.GetService(typeof(AbstractEntityValidator<DummyEntity>)), Times.Once);
        serviceProdiver.VerifyNoOtherCalls();
    }
}

internal class DummyEntity : EntityBase
{
    public bool IsValid { get; set; }

    public DummyEntity(bool isValid)
    {
        this.IsValid = isValid;
    }
}

internal class DummyEntityValidator : AbstractEntityValidator<DummyEntity>
{
    public override Task<DomainValidationResult> ValidateAsync(DummyEntity entity)
    {
        DomainValidationResult result = new();

        if (!entity.IsValid)
            result.AddError("Dummy invalid.");

        return Task.FromResult(result);
    }
}
