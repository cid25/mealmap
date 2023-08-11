using FluentAssertions;
using Mealmap.Domain;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;
using Moq;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess;

public class DeferredDomainValidatorTests
{
    [Fact]
    public async void ValidateEntitiesAsync_WhenNoEntities_CompletesWithoutExceptionOrCallToMediator()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        DeferredDomainValidator validator = new(mediatorMock.Object);
        var noEntities = new List<Meal>().AsReadOnly();

        // Act
        var act = async () => await validator.ValidateEntitiesAsync(noEntities);

        // Assert
        await act.Should().NotThrowAsync();
        mediatorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ValidateEntitiesAsync_WhenEntitiesContainValidMeal_CallsMediatorAndCompletesWithoutException()
    {
        // Arrange
        DomainValidationResult validValidationResultStub = new();
        var mediatorStub = new Mock<IMediator>();
        mediatorStub.Setup(m => m.Send(It.IsAny<ValidationRequest<Meal>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validValidationResultStub));
        DeferredDomainValidator validator = new(mediatorStub.Object);

        // Act
        await validator.ValidateEntitiesAsync(
            new List<Meal>() { new Meal(DateOnly.FromDateTime(DateTime.Now)) }.AsReadOnly());

        // Assert
        mediatorStub.Verify(m => m.Send(It.IsAny<ValidationRequest<Meal>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ValidateEntitiesAsync_WhenEntitiesContainInvalidMeal_ThrowsDomainValidationException()
    {
        // Arrange
        DomainValidationResult validationResult = new();
        validationResult.AddError(String.Empty);
        var mediatorThrowingException = Mock.Of<IMediator>(m =>
            m.Send(It.IsAny<ValidationRequest<Meal>>(), It.IsAny<CancellationToken>())
                == Task.FromResult(validationResult));
        DeferredDomainValidator validator = new(mediatorThrowingException);

        // Act
        var act = () => validator.ValidateEntitiesAsync(
            new List<Meal>() { new Meal(DateOnly.FromDateTime(DateTime.Now)) }.AsReadOnly());

        // Assert
        act.Should().ThrowAsync<DomainValidationException>();
    }
}
