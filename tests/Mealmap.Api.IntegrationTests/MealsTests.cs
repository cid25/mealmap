using System.Net.Http.Json;
using Mealmap.Api.Meals;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.IntegrationTests;

[Collection("InSequence")]
[Trait("Target", "Pipeline")]
public class MealsTests
{
    [Fact]
    public async void GetMeals_WhenMealsExist_ReturnsJsonAndStatusOk()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void GetMeals_WhenMealsExist_ReturnsMealDtos()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals");

        // Assert
        var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<MealDTO>>();
        dtos.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async void GetMeals_WhenNoMealsExist_ReturnsNoContent()
    {
        // Arrange
        DatabaseSeeder.Init(withData: false);
        var factory = new MockableWebApplicationFactory(null);

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/meals");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void GetMeals_WhenNoMealsFound_ReturnsNoContent()
    {
        // Arrange
        DatabaseSeeder.Init();
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
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals/" + existingMeal.Id);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void GetMeal_ReturnsMealDtoOfCorrectId()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/meals/" + existingMeal.Id);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<MealDTO>();
        result!.Id.Should().Be(existingMeal.Id);
    }

    [Fact]
    public async void PostMeal_WhenDataValid_CreatesAndReturnsMealWithId()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var freeDiningDate = DatabaseSeeder.Meals.Max(m => m.DiningDate).AddDays(1);
        MealDTO meal = new()
        {
            DiningDate = freeDiningDate,
            Courses = [
                new CourseDTO()
                {
                    Index = 1,
                    DishId = (DatabaseSeeder.GetRandomDish()).Id,
                    MainCourse = true,
                    Attendees = 2
                }
            ]
        };
        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/meals/", meal);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<MealDTO>();
        result!.Id.Should().NotBeNull().And.NotBeEmpty();
        context.Find<Meal>(result.Id).Should().NotBeNull();
    }

    [Fact]
    public async void PostMeal_WhenDishInCourseNotExisting_ReturnsBadRequest()
    {
        // Arrange
        var factory = new MockableWebApplicationFactory(null);

        var nonExistingDishId = Guid.NewGuid();
        MealDTO meal = new()
        {
            DiningDate = DateOnly.MinValue,
            Courses = [
                new CourseDTO()
                {
                    Index = 1,
                    DishId = nonExistingDishId,
                    MainCourse = true,
                    Attendees = 2
                }
            ]
        };
        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/meals/", meal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void PutMeal_WhenRequestValid_ReturnsOkAndUpdatedMeal()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = context.Find<Meal>(DatabaseSeeder.GetRandomMeal().Id);
        context.ChangeTracker.Clear();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", existingMeal!.Version.AsString());

        var newDate = DateOnly.MaxValue;
        MealDTO updatedMeal = new()
        {
            DiningDate = newDate
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/meals/{existingMeal.Id}", updatedMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MealDTO>();
        result!.DiningDate.Should().Be(newDate);
    }

    [Fact]
    public async void PutMeal_WhenRequestValid_UpdatesMeal()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = context.Find<Meal>(DatabaseSeeder.GetRandomMeal().Id);
        context.ChangeTracker.Clear();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", existingMeal!.Version.AsString());

        var newDate = DateOnly.MaxValue;
        MealDTO updatedMeal = new()
        {
            DiningDate = newDate
        };

        // Act
        await client.PutAsJsonAsync($"/api/meals/{existingMeal.Id}", updatedMeal);

        // Assert
        var savedMeal = context.Find<Meal>(existingMeal.Id);
        savedMeal!.DiningDate.Should().Be(newDate);
    }

    [Fact]
    public async void PutMeal_WhenIfMatchHeaderNotSet_ReturnsPreconditionRequired()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.GetRandomMeal();
        var dummyMeal = new MealDTO() { DiningDate = DateOnly.MaxValue };

        // Act
        var response = await factory.CreateClient().PutAsJsonAsync($"/api/meals/{existingMeal.Id}", dummyMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.PreconditionRequired);
    }

    [Fact]
    public async void PutMeal_WhenIfMatchHeaderNotMatching_ReturnsPreconditionFailed()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.GetRandomMeal();
        var dummyMeal = new MealDTO() { DiningDate = DateOnly.MaxValue };
        var randomEtag = "ABABABA=";

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", randomEtag);

        // Act
        var response = await client.PutAsJsonAsync($"/api/meals/{existingMeal.Id}", dummyMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.PreconditionFailed);
    }

    [Fact]
    public async void PutMeal_WhenIfMatchHeaderInvalidFormat_ReturnsPreconditionFailed()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.GetRandomMeal();
        var dummyMeal = new MealDTO() { DiningDate = DateOnly.MaxValue };
        var invalidEtag = "bogus";

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", invalidEtag);

        // Act
        var response = await client.PutAsJsonAsync($"/api/meals/{existingMeal.Id}", dummyMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.PreconditionFailed);
    }

    [Fact]
    public async void PutMeal_WhenMealNotExisting_ReturnsNotFound()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var client = factory.CreateClient();
        var dummyEtag = "ABABABAB=";
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", dummyEtag);

        var dummyMeal = new MealDTO() { DiningDate = DateOnly.MaxValue };

        var nonExistingMealId = Guid.NewGuid();

        // Act
        var response = await client.PutAsJsonAsync($"/api/meals/{nonExistingMealId}", dummyMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async void PutMeal_WhenRequestInvalid_ReturnsBadRequest()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = context.Find<Meal>(DatabaseSeeder.GetRandomMeal().Id);

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", existingMeal!.Version.AsString());

        var nonExistingDishId = Guid.NewGuid();
        MealDTO updatedMeal = new()
        {
            DiningDate = existingMeal!.DiningDate,
            Courses = [
                new()
                {
                    Index = 1,
                    DishId = nonExistingDishId,
                    MainCourse = true,
                    Attendees = 2
                }
            ]
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/meals/{existingMeal.Id}", updatedMeal);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void DeleteMeal_DeletesMeal()
    {
        // Arrange
        var context = DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var existingMeal = DatabaseSeeder.Meals.First();

        // Act
        await factory.CreateClient().DeleteAsync("/api/meals/" + existingMeal.Id);

        // Assert
        context.Find<Meal>(existingMeal.Id).Should().BeNull();
    }

    [Fact]
    public async void DeleteMeal_ReturnsJsonAndStatusOk()
    {
        // Arrange
        DatabaseSeeder.Init();
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
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var randomId = Guid.NewGuid();

        // Act
        var response = await factory.CreateClient().DeleteAsync("/api/meals/" + randomId);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
