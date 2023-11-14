using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;

namespace Mealmap.Api.IntegrationTests;

[Collection("InSequence")]
[Trait("Target", "Pipeline")]
public class MealsTests
{
    private readonly MealmapDbContext _context;

    public MealsTests()
    {
        _context = DatabaseSeeder.Init();
    }

    [Fact]
    public async void GetMeals_WhenMealsFound_ReturnsJsonAndStatusOk()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void GetMeals_WhenNoMealsFound_ReturnsNoContent()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);
        var (blankRangeStartDate, blankRangeEndDate) = ("0001-01-01", "0001-01-07");

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/meals?fromDate={blankRangeStartDate}&toDate={blankRangeEndDate}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void GetMeal_ReturnsJsonAndStatusOk()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals/" + existingMeal.Id);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void DeleteMeal_DeletesMeal()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        await factory.CreateClient().DeleteAsync("/api/meals/" + existingMeal.Id);

        // Assert
        _context.Find<Meal>(existingMeal.Id).Should().BeNull();
    }

    [Fact]
    public async void DeleteMeal_ReturnsJsonAndStatusOk()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        var response = await factory.CreateClient().DeleteAsync("/api/meals/" + existingMeal.Id);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void DeleteMeal_WhenMealDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        var randomId = Guid.NewGuid();

        // Act
        var response = await factory.CreateClient().DeleteAsync("/api/meals/" + randomId);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
