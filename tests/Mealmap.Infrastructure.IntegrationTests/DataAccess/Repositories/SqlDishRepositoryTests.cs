using System.Diagnostics;
using FluentAssertions;
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
        Dish dishWithoutImage = new("Krabby Patty", "The fishiest burger in town.", 2);
        dishWithoutImage.AddIngredient(4, "Slice", "Old bread");
        dishWithoutImage.AddIngredient(2, "Piece", "Unidentifiable meat");
        dishWithoutImage.AddIngredient(20, "Mililiter", "Fishy sauce");
        _dishes[0] = dishWithoutImage;

        Dish dishWithImage = new("Sailors Surprise", "The darkest, wettest dream of every boatsman.", 4);
        dishWithImage.SetImage(new byte[] { 0x01 }, "image/jpeg");
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
        var expectedCount = _repository.dbSet.Count();

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
        var aGuid = Guid.NewGuid();
        Dish aValidDish = new(aGuid, "Salty Sea Dog", null, 1);

        _repository.Add(aValidDish);

        _dbContext.ChangeTracker.Clear();
        _dbContext.Find<Dish>(aGuid).Should().NotBeNull();
    }

    [Fact]
    public void Add_WhenDishHasIngredients_CreatesIngredients()
    {
        var aGuid = Guid.NewGuid();
        Dish aDish = new(aGuid, "Salty Sea Dog", null, 1);
        aDish.AddIngredient(1, "Kilogram", "Sausages");
        aDish.AddIngredient(0.5m, "Liter", "Ketchup");
        aDish.AddIngredient(0.3m, "Liter", "Mustard");

        _repository.Add(aDish);

        _dbContext.ChangeTracker.Clear();
        _dbContext.Find<Dish>(aGuid)!.Ingredients.Should().HaveCount(3);
    }

    [Fact]
    public void Update_WhenDishDisconnected_ThrowsInvalidOperationException()
    {
        var initialDish = _dbContext.Find<Dish>(_dishes[0].Id);
        Dish aDisconnectedDish = new(initialDish!.Id, initialDish.Name, initialDish.Description, initialDish.Servings);

        Action act = () => _repository.Update(aDisconnectedDish);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Update_WhenImageExists_RetainsImage()
    {
        var dishWithImage = _dbContext.Find<Dish>(_dishes[1].Id);

        var anotherDishName = "Tuna Supreme";
        dishWithImage!.Name = anotherDishName;
        _repository.Update(dishWithImage);

        _dbContext.ChangeTracker.Clear();
        var dish = _dbContext.Find<Dish>(_dishes[1].Id);
        dish!.Image.Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenConcurrentUpdate_ThrowsConcurrentUpdateException()
    {
        var dish = _dbContext.Find<Dish>(_dishes[1].Id);
        dish!.Name = "Tuna Supreme";

        _dbContext.Database.ExecuteSqlRaw("UPDATE [mealmap].[dish] SET [Name] = 'Golden Seahorse' WHERE [Id] = '" + dish.Id + "';");
        Action act = () => _repository.Update(dish);

        act.Should().Throw<ConcurrentUpdateException>();
    }

    [Fact]
    public void Update_WhenExplicitVersionNotMatchingDatabase_ThrowsConcurrentUpdateException()
    {
        var dish = _dbContext.Find<Dish>(_dishes[1].Id);

        var nonMatchingVersion = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xD4 };
        dish!.Version.Set(nonMatchingVersion);
        dish.Name = "Tuna Supreme";

        Action act = () => _repository.Update(dish);

        act.Should().Throw<ConcurrentUpdateException>();
    }

    [Fact]
    public void Update_WhenIngredientAdded_AddsIngredientAndUpsDishVersion()
    {
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count;
        var originalVersion = dish.Version;

        dish!.AddIngredient(1, "Pinch", "Pepper");
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount + 1);
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void Update_WhenIngredientRemoved_RemovesIngredientAndUpsDishVersion()
    {
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);
        var originalCount = dish!.Ingredients.Count;
        var originalVersion = dish.Version;

        dish!.RemoveIngredient(dish.Ingredients.First());
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Ingredients.Should().HaveCount(originalCount - 1);
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void Update_WhenImageReplaced_ReplacesImage()
    {
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);

        var imageContent = new byte[] { 0x02 };
        dish!.SetImage(imageContent, "image/jpeg");
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Image!.Content.Should().Equal(imageContent);
    }

    [Fact]
    public void Update_WhenImageRemoved_DeletesImage()
    {
        var dish = _dbContext.Find<Dish>(_dishes[0].Id);

        dish!.RemoveImage();
        _repository.Update(dish);

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Dish>(dish.Id);
        result!.Image.Should().BeNull();
    }

    [Fact]
    public void Remove_RemovesEntry()
    {
        var expectedCount = _repository.dbSet.Count();
        var dish = _repository.dbSet.First();

        _repository.Remove(dish!);

        _repository.dbSet.Count().Should().Be(expectedCount - 1);
    }
}
