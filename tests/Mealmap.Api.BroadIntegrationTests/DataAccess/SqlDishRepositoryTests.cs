using System.Diagnostics;
using FluentAssertions;
using Mealmap.Domain.DishAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess;

[Collection("InSequence")]
[Trait("Target", "Database")]
public class SqlDishRepositoryTests
{
    private readonly MealmapDbContext _dbContext;
    private readonly SqlDishRepository _repository;
    private readonly Dish[] _dishes;

    public SqlDishRepositoryTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
            .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
            .LogTo(msg => Debug.WriteLine(msg))
            .EnableSensitiveDataLogging()
            .Options;
        _dbContext = new MealmapDbContext(dbOptions);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _dishes = new Dish[2];
        seedData();
        _dbContext.ChangeTracker.Clear();

        _repository = new SqlDishRepository(_dbContext);
    }

    private void seedData()
    {
        var dishWithoutImage = new Dish("Krabby Patty")
        {
            Description = "The fishiest burger in town.",
            Servings = 2,
        };
        dishWithoutImage.AddIngredient(4, "Slice", "Old bread");
        dishWithoutImage.AddIngredient(2, "Piece", "Unidentifiable meat");
        dishWithoutImage.AddIngredient(20, "Mililiter", "Fishy sauce");
        _dishes[0] = dishWithoutImage;

        var dishWithImage = new Dish("Sailors Surprise")
        {
            Description = "The darkest, wettest dream of every boatsman.",
            Servings = 4,
            Image = new DishImage(new byte[] { 0x01 }, "image/jpeg")
        };
        dishWithImage.AddIngredient(800, "Mililiter", "Seawater");
        dishWithImage.AddIngredient(6, "Piece", "Sea cucumber");
        dishWithImage.AddIngredient(8, "Piece", "Crab meat");
        dishWithImage.AddIngredient(1, "Pinch", "Salt");
        _dishes[1] = dishWithImage;

        _dbContext.AddRange(_dishes);
        _dbContext.SaveChanges();
    }


    [Fact]
    public void GetAll_ReturnsAllDishes()
    {
        var expectedCount = _dbContext.Dishes.Count();

        var result = _repository.GetAll();

        result.Should().NotBeEmpty().And.HaveCount(expectedCount);
    }

    [Fact]
    public void GetSingleById_WhenIdNonExisting_ReturnsNull()
    {
        const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
        var result = _repository.GetSingleById(new Guid(nonExistingGuid));

        result.Should().BeNull();
    }

    [Fact]
    public void GetSingleById_WhenIdExists_ReturnsDish()
    {
        var existingId = _dishes[0].Id;
        var result = _repository.GetSingleById(existingId);

        result.Should().NotBeNull();
        result!.Name.Should().Be(_dishes[0].Name);
    }

    [Fact]
    public void Add_WhenDishValid_CreatesEntry()
    {
        var someGuid = Guid.NewGuid();
        Dish dish = new(someGuid, "Salty Sea Dog");

        _repository.Add(dish);

        _dbContext.ChangeTracker.Clear();
        _dbContext.Dishes.Find(someGuid).Should().NotBeNull();
    }

    [Fact]
    public void Add_WhenDishHasIngredients_CreatesIngredients()
    {
        var someGuid = Guid.NewGuid();
        Dish dish = new(someGuid, "Salty Sea Dog");
        dish.AddIngredient(1, "Kilogram", "Sausages");
        dish.AddIngredient(0.5m, "Liter", "Ketchup");
        dish.AddIngredient(0.3m, "Liter", "Mustard");

        _repository.Add(dish);

        _dbContext.ChangeTracker.Clear();
        _dbContext.Dishes.Find(someGuid)!.Ingredients.Should().HaveCount(3);
    }

    [Fact]
    public void Update_WhenGivenDisconnectedDish_ThrowsInvalidOperationException()
    {
        var initialDish = _dbContext.Dishes.Find(_dishes[0].Id);
        var anotherDishName = "Tuna Supreme";
        var disconnectedDish = new Dish(initialDish!.Id, anotherDishName) { Version = initialDish!.Version };

        Action act = () => _repository.Update(disconnectedDish);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Update_WhenImageExists_RetainsImage()
    {
        var dishWithImage = _dbContext.Dishes.Find(_dishes[1].Id);

        var anotherDishName = "Tuna Supreme";
        dishWithImage!.Name = anotherDishName;
        _repository.Update(dishWithImage);

        _dbContext.ChangeTracker.Clear();
        var dish = _dbContext.Dishes.Find(_dishes[1].Id);
        dish!.Image.Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenConcurrentUpdate_ThrowsConcurrentUpdateException()
    {
        var dish = _dbContext.Dishes.Find(_dishes[1].Id);
        dish!.Name = "Tuna Supreme";

        _dbContext.Database.ExecuteSqlRaw("UPDATE [mealmap].[dish] SET [Name] = 'Golden Seahorse' WHERE [Id] = '" + _dishes[1].Id + "';");
        Action act = () => _repository.Update(dish);

        act.Should().Throw<ConcurrentUpdateException>();
    }

    [Fact]
    public void Update_WhenVersionNotMatchingDatabase_ThrowsConcurrentUpdateException()
    {
        var dish = _dbContext.Dishes.Find(_dishes[1].Id);

        var nonMatchingVersion = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xD4 };
        dish!.Version = nonMatchingVersion;
        dish.Name = "Tuna Supreme";

        Action act = () => _repository.Update(dish);

        act.Should().Throw<ConcurrentUpdateException>();
    }

    [Fact]
    public void Update_WhenIngredientAdded_AddsIngredientAndUpsDishVersion()
    {
        var dish = _dbContext.Dishes.Find(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count();
        var originalVersion = dish.Version;

        dish!.AddIngredient(1, "Pinch", "Pepper");
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Dishes.Find(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount + 1);
        result.Version.Should().NotEqual(originalVersion);
    }

    [Fact]
    public void Update_WhenIngredientRemoved_RemovesIngredientAndUpsDishVersion()
    {
        var dish = _dbContext.Dishes.Find(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count();
        var originalVersion = dish.Version;

        dish!.RemoveIngredient(dish.Ingredients.First());
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Dishes.Find(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount - 1);
        result.Version.Should().NotEqual(originalVersion);
    }

    [Fact]
    public void Update_WhenImageReplaced_ReplacesImage()
    {
        var dish = _dbContext.Dishes.Find(_dishes[0].Id);

        var imageContent = new byte[] { 0x02 };
        dish!.Image = new DishImage(imageContent, "image/jpeg");
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Dishes.Find(dish.Id);
        result!.Image!.Content.Should().Equal(imageContent);
    }

    [Fact]
    public void Update_WhenImageRemoved_DeletesImage()
    {
        var dish = _dbContext.Dishes.Find(_dishes[0].Id);
        dish!.Image = null;

        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Dishes.Find(dish.Id);
        result!.Image.Should().BeNull();
    }

    [Fact]
    public void Remove_RemovesEntry()
    {
        var expectedCount = _dbContext.Dishes.Count();
        var dish = _dbContext.Dishes.First();

        _repository.Remove(dish!);

        _dbContext.Dishes.Count().Should().Be(expectedCount - 1);
    }
}
