using SystemHeaders = System.Net.Http.Headers;

namespace Mealmap.Api.IntegrationTests;


[Collection("InSequence")]
public class DishesTests
{

    [Fact]
    public async void PutDishImage_WhenImageUploaded_ReturnsNeitherBodyNorContentType()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var dummyContent = new ByteArrayContent(new byte[1]);
        dummyContent.Headers.ContentType = SystemHeaders.MediaTypeHeaderValue.Parse("image/jpeg");
        var existingDish = DatabaseSeeder.Dishes.First();

        // Act
        var response = await factory.CreateClient().PutAsync("/api/dishes/" + existingDish.Id + "/image", dummyContent);


        // Assert
        response.Content.Headers.ContentLength.Should().Be(0);
        response.Content.Headers.ContentType.Should().BeNull();
    }

    [Fact]
    public async void PutDishImage_WhenFileNotSupportedImageType_ReturnsUnsupportedMediaType()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var unsupportedImageType = "image/tiff";
        var dummyContent = new ByteArrayContent(new byte[1]);
        dummyContent.Headers.ContentType = SystemHeaders.MediaTypeHeaderValue.Parse(unsupportedImageType);
        var existingDish = DatabaseSeeder.Dishes.First();

        // Act
        var response = await factory.CreateClient().PutAsync($"/api/dishes/{existingDish.Id}/image", dummyContent);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async void GetDishImage_ReturnsCorrectContentTypeAndStatusOk()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var dishWithImage = DatabaseSeeder.Dishes.Where(d => d.Image != null).First();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/dishes/" + dishWithImage.Id + "/image");

        // Assert
        response.Content.Headers.ContentType!.MediaType.Should().Be(dishWithImage.Image!.ContentType);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void GetDishImage_WhenDishHasNoImage_ReturnsNoContent()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var dishWithoutImage = DatabaseSeeder.Dishes.Where(d => d.Image == null).First();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/dishes/" + dishWithoutImage.Id + "/image");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void GetDishImage_WhenDishDoesntExist_ReturnsNotFound()
    {
        // Arrange
        DatabaseSeeder.Init();
        var factory = new MockableWebApplicationFactory(null);

        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/dishes/" + nonExistingId + "/image");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
