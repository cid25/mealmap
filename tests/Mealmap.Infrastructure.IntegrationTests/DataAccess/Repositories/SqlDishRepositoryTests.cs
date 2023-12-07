using System.Diagnostics;
using Mealmap.Domain.DishAggregate;
using Mealmap.Infrastructure.DataAccess;
using Mealmap.Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess.Repositories;

[Collection("InSequence")]
[Trait("Target", "Database")]
public class SqlDishRepositoryTests
{
    private readonly MealmapDbContext _dbContext;
    private readonly SqlDishRepository _repository;
    private readonly Dish[] _dishes;

    public SqlDishRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("settings.json")
            .Build();
        var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
            .UseSqlServer(
                configuration.GetConnectionString("MealmapDb"),
                b =>
                {
                    b.MigrationsAssembly("Mealmap.Migrations");
                    b.EnableRetryOnFailure();
                }
            )
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
        Dish dishWithoutImage = new("Krabby Patty", "The fishiest burger in town.", 2);
        dishWithoutImage.AddIngredient(4, "Slice", "Old bread");
        dishWithoutImage.AddIngredient(2, "Piece", "Unidentifiable meat");
        dishWithoutImage.AddIngredient(20, "Mililiter", "Fishy sauce");
        _dishes[0] = dishWithoutImage;

        Dish dishWithImage = new("Sailors Surprise", "The darkest, wettest dream of every boatsman.", 4);
        dishWithImage.SetImage([0x01], "image/jpeg");
        dishWithImage.AddIngredient(800, "Mililiter", "Seawater");
        dishWithImage.AddIngredient(6, "Piece", "Sea cucumber");
        dishWithImage.AddIngredient(8, "Piece", "Crab meat");
        dishWithImage.AddIngredient(1, "Pinch", "Salt");
        _dishes[1] = dishWithImage;

        _dbContext.AddRange(_dishes);
        _dbContext.SaveChanges();
    }

    [Fact]
    public void GetSingleById_WhenIdNonExisting_ReturnsNull()
    {
        // Arrange
        const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";

        // Act
        var result = _repository.GetSingleById(new Guid(nonExistingGuid));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetSingleById_WhenIdExists_ReturnsDish()
    {
        // Arrage
        var existingId = _dishes[0].Id;

        // Act
        var result = _repository.GetSingleById(existingId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(_dishes[0].Name);
    }

    [Fact]
    public void SaveOnAdd_WhenDishValid_CreatesEntry()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        Dish aValidDish = new(aGuid, "Salty Sea Dog", null, 1);

        // Act
        _repository.Add(aValidDish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        _dbContext.Find<Dish>(aGuid).Should().NotBeNull();
    }

    [Fact]
    public void SaveOnAdd_WhenDishHasIngredients_CreatesIngredients()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        Dish aDish = new(aGuid, "Salty Sea Dog", null, 1);
        aDish.AddIngredient(1, "Kilogram", "Sausages");
        aDish.AddIngredient(0.5m, "Liter", "Ketchup");
        aDish.AddIngredient(0.3m, "Liter", "Mustard");

        // Act
        _repository.Add(aDish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        _dbContext.Find<Dish>(aGuid)!.Ingredients.Should().HaveCount(3);
    }

    [Fact]
    public void SaveOnUpdate_WhenDishDisconnected_ThrowsInvalidOperationException()
    {
        // Arrange
        var initialDish = _dbContext.Find<Dish>(_dishes[0].Id);
        Dish aDisconnectedDish = new(initialDish!.Id, initialDish.Name, initialDish.Description, initialDish.Servings);

        // Act
        Action act = () => _repository.Update(aDisconnectedDish);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SaveOnUpdate_WhenConcurrentUpdate_ThrowsException()
    {
        // Arrange
        var dish = _dbContext.Find<Dish>(_dishes[1].Id);
        dish!.Name = "Tuna Supreme";

        // Act
        _dbContext.Database.ExecuteSqlRaw("UPDATE [mealmap].[dish] SET [Name] = 'Golden Seahorse' WHERE [Id] = '" + dish.Id + "';");
        _repository.Update(dish);
        Action act = () => _dbContext.SaveChanges();

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void SaveOnUpdate_WhenExplicitVersionNotMatchingDatabase_ThrowsException()
    {
        // Arrange
        var dish = _dbContext.Find<Dish>(_dishes[1].Id);
        var nonMatchingVersion = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xD4 };
        dish!.Version.Set(nonMatchingVersion);
        dish.Name = "Tuna Supreme";

        // Act
        _repository.Update(dish);
        Action act = () => _dbContext.SaveChanges();

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void SaveOnUpdate_WhenIngredientAdded_AddsIngredientAndUpsDishVersion()
    {
        // Arrange
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count;
        var originalVersion = dish.Version;

        // Act
        dish!.AddIngredient(1, "Pinch", "Pepper");
        _repository.Update(dish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount + 1);
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void SaveOnUpdate_WhenIngredientRemoved_RemovesIngredientAndUpsDishVersion()
    {
        // Arrange
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count;
        var originalVersion = dish.Version;

        // Act
        dish!.RemoveIngredient(dish.Ingredients.First());
        _repository.Update(dish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount - 1);
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void SaveOnUpdate_WhenImageReplaced_ReplacesImage()
    {
        // Arrange
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);
        var imageContent = new byte[] { 0x02 };
        dish!.SetImage(imageContent, "image/jpeg");

        // Act
        _repository.Update(dish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Image!.Content.Should().Equal(imageContent);
    }

    [Fact]
    public void SaveOnUpdate_WhenImageRemoved_DeletesImage()
    {
        // Arrange 
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);

        // Act
        dish!.RemoveImage();
        _repository.Update(dish);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Image.Should().BeNull();
    }

    [Fact]
    public void SaveOnRemove_RemovesEntry()
    {
        // Arrange
        var expectedCount = _repository.dbSet.Count();
        var dish = _repository.dbSet.First();

        // Act
        _repository.Remove(dish!);
        _dbContext.SaveChanges();

        // Assert
        _repository.dbSet.Should().HaveCount(expectedCount - 1);
    }
}
