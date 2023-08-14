using Mealmap.Domain;
using Mealmap.Domain.Common;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess;

public class DeferredDomainValidatorTests
{
    [Fact]
    public async void ValidateEntitiesAsync_WhenNoEntities_CompletesWithoutExceptionOrCallToMediator()
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
    public async void ValidateEntitiesAsync_WhenEntitiesContainValidMeal_CallsEntityValidatorAndCompletesWithoutException()
    {
        // Arrange
        DomainValidationResult validValidationResult = new();
        var mealValidator = new Mock<IEntityValidator<Meal>>();
        mealValidator.Setup(m => m.ValidateAsync(It.IsAny<Meal>())).Returns(Task.FromResult(validValidationResult));
        var serviceProdiver = new Mock<IServiceProvider>();
        serviceProdiver.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mealValidator.Object);
        DeferredDomainValidator deferredValidator = new(serviceProdiver.Object);

        // Act
        await deferredValidator.ValidateEntitiesAsync(new List<EntityBase>() {
            new Meal(DateOnly.FromDateTime(DateTime.Now)) });

        // Assert
        mealValidator.Verify(m => m.ValidateAsync(It.IsAny<Meal>()), Times.Once);
    }

    [Fact]
    public void ValidateEntitiesAsync_WhenEntitiesContainInvalidMeal_ThrowsDomainValidationException()
    {
        // Arrange
        DomainValidationResult invalidValidationResult = new();
        invalidValidationResult.AddError(String.Empty);
        var mealValidator = new Mock<IEntityValidator<Meal>>();
        mealValidator.Setup(m => m.ValidateAsync(It.IsAny<Meal>())).Returns(Task.FromResult(invalidValidationResult));
        var serviceProdiver = new Mock<IServiceProvider>();
        serviceProdiver.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mealValidator.Object);
        DeferredDomainValidator deferredValidator = new(serviceProdiver.Object);

        // Act
        var act = () => deferredValidator.ValidateEntitiesAsync(new List<EntityBase>() {
            new Meal(DateOnly.FromDateTime(DateTime.Now)) });

        // Assert
        act.Should().ThrowAsync<DomainValidationException>();
    }
}
