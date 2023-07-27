using FluentAssertions;
using Mealmap.DataAccess;
using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.BroadIntegrationTests;

[Collection("InSequence")]
[Trait("Target", "Database")]
public class SqlDishRepositoryTests
{
    private readonly MealmapDbContext _dbContext;
    private readonly SqlDishRepository _repository;

    public SqlDishRepositoryTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
            .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
            .Options;
        _dbContext = new MealmapDbContext(dbOptions);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        seedData();
        Helpers.DetachAllEntities(_dbContext);

        _repository = new SqlDishRepository(_dbContext);
    }

    private void seedData()
    {
        _dbContext.Dishes.Add(new Dish("Krabby Patty")
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Description = "The fishiest burger in town.",
            Servings = 2,
            Ingredients = new()
            {
                new Ingredient(4, new UnitOfMeasurement(UnitOfMeasurementCodes.Slice), "Old bread"),
                new Ingredient(2, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Unidentifiable meat"),
                new Ingredient(20, new UnitOfMeasurement(UnitOfMeasurementCodes.Mililiter), "Fishy sauce"),
            }
        });
        _dbContext.Dishes.Add(new Dish("Sailors Surprise")
        {
            Id = new Guid("00000000-0000-0000-0000-000000000010"),
            Description = "The darkest, wettest dream of every boatsman.",
            Servings = 4,
            Ingredients = new()
            {
                new Ingredient(800, new UnitOfMeasurement(UnitOfMeasurementCodes.Mililiter), "Seawater"),
                new Ingredient(6, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Sea cucumber"),
                new Ingredient(8, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Crab meat"),
                new Ingredient(1, new UnitOfMeasurement(UnitOfMeasurementCodes.Pinch), "Salt"),
            }
        });
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
        const string existingGuid = "00000000-0000-0000-0000-000000000001";
        var result = _repository.GetSingleById(new Guid(existingGuid));

        result.Should().NotBeNull();
        result!.Name.Should().Be("Krabby Patty");
    }

    [Fact]
    public void Add_WhenDishValid_CreatesEntry()
    {
        const string someDishName = "Salty Sea Dog";
        Guid someGuid = Guid.NewGuid();
        Dish dish = new(someDishName) { Id = someGuid };

        _repository.Add(dish);

        _dbContext.Dishes.First(x => x.Id == someGuid).Should().NotBeNull();
    }

    [Fact]
    public void Add_WhenDishHasIngredients_CreatesIngredients()
    {
        const string someDishName = "Salty Sea Dog";
        Guid someGuid = Guid.NewGuid();
        Dish dish = new(someDishName) { Id = someGuid };
        dish.AddIngredient(1, "Kilogram", "Sausages");
        dish.AddIngredient(0.5m, "Liter", "Ketchup");
        dish.AddIngredient(0.3m, "Liter", "Mustard");

        _repository.Add(dish);

        _dbContext.Dishes.First(x => x.Id == someGuid).Ingredients.Should().HaveCount(3);
    }

    [Fact]
    public void Update_WhenGivenDisconnectedDish_UpdatesEntryAndUpsDishVersion()
    {
        const string someDishName = "Salty Sea Dog";
        Guid guid = Guid.NewGuid();
        Dish initialDish = new(someDishName) { Id = guid };
        _dbContext.Dishes.Add(initialDish);
        _dbContext.SaveChanges();
        _dbContext.Remove(initialDish);

        const string anotherDishName = "Tuna Supreme";
        var newDisconnectedDish = new Dish(anotherDishName) { Id = guid };
        _repository.Update(newDisconnectedDish);

        var dish = _dbContext.Dishes.First(d => d.Id == initialDish.Id);
        dish!.Name.Should().Be(anotherDishName);
        dish!.Version.Should().NotEqual(initialDish.Version);
    }

    [Fact]
    public void Update_WhenImageExists_RetainsImage()
    {
        const string someDishName = "Salty Sea Dog";
        Guid guid = Guid.NewGuid();
        var someImage = new DishImage(new byte[] { 0x01 }, "image/jpeg");
        Dish initialDish = new(someDishName) { Id = guid, Image = someImage };
        _dbContext.Dishes.Add(initialDish);
        _dbContext.SaveChanges();
        _dbContext.Remove(initialDish);

        const string anotherDishName = "Tuna Supreme";
        var newDisconnectedDish = new Dish(anotherDishName) { Id = guid };
        _repository.Update(newDisconnectedDish);

        var dish = _dbContext.Dishes.First(d => d.Id == initialDish.Id);
        dish!.Image.Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenIngredientAdded_AddsIngredientAndUpsDishVersion()
    {
        var originalDish = _dbContext.Dishes.First();
        var originalIngredientCount = originalDish.Ingredients!.Count;
        var originalDishVersion = originalDish.Version;
        _dbContext.Entry(originalDish).State = EntityState.Detached;
        var adaptedDish = new Dish(originalDish.Name)
        {
            Id = originalDish.Id,
            Description = originalDish.Description,
            Servings = originalDish.Servings,
            Ingredients = originalDish.Ingredients!.ToArray<Ingredient>().ToList<Ingredient>()
        };

        adaptedDish.Ingredients!.Add(new Ingredient(1, UnitOfMeasurementCodes.Pinch, "Pepper"));
        _repository.Update(adaptedDish);

        var dish = _dbContext.Dishes.First(d => d.Id == originalDish.Id);
        dish.Ingredients.Should().HaveCount(originalIngredientCount + 1);
        dish.Version.Should().NotEqual(originalDishVersion);
    }

    [Fact]
    public void Update_WhenIngredientRemoved_RemovesIngredientAndUpsDishVersion()
    {
        var originalDish = _dbContext.Dishes.First(d => d.Ingredients != null && d.Ingredients.Count > 2);
        var originalIngredientCount = originalDish.Ingredients!.Count;
        var originalDishVersion = originalDish.Version;
        _dbContext.Entry(originalDish).State = EntityState.Detached;
        var adaptedDish = new Dish(originalDish.Name)
        {
            Id = originalDish.Id,
            Description = originalDish.Description,
            Servings = originalDish.Servings,
            Ingredients = originalDish.Ingredients!.ToArray<Ingredient>().ToList<Ingredient>()
        };

        adaptedDish.Ingredients.RemoveAt(0);
        _repository.Update(adaptedDish);

        var dish = _dbContext.Dishes.First(d => d.Id == originalDish.Id);
        dish.Ingredients.Should().HaveCount(originalIngredientCount - 1);
        dish.Version.Should().NotEqual(originalDishVersion);
    }

    [Fact]
    public void Update_WhenImageReplaced_ReplacesImage()
    {
        var originalDish = _dbContext.Dishes.First();
        originalDish.Image = new DishImage(new byte[] { 0x01 }, "image/jpeg");
        _dbContext.SaveChanges();
        _dbContext.Entry(originalDish).State = EntityState.Detached;

        var imageContent = new byte[] { 0x02 };
        var adaptedDish = new Dish(originalDish.Name)
        {
            Id = originalDish.Id,
            Image = new DishImage(imageContent, "image/jpeg")
        };
        _repository.Update(adaptedDish, retainImage: false);
        _dbContext.Entry(adaptedDish).State = EntityState.Detached;

        var dish = _dbContext.Dishes.Find(adaptedDish.Id);
        dish!.Image!.Content.Should().Equal(imageContent);
    }

    [Fact]
    public void Update_WhenImageRemoved_DeletesImage()
    {
        var originalDish = _dbContext.Dishes.First();
        originalDish.Image = new DishImage(new byte[] { 0x01 }, "image/jpeg");
        _dbContext.SaveChanges();
        _dbContext.Entry(originalDish).State = EntityState.Detached;

        var adaptedDish = new Dish(originalDish.Name)
        {
            Id = originalDish.Id,
            Image = null
        };
        _repository.Update(adaptedDish, retainImage: false);
        _dbContext.Entry(adaptedDish).State = EntityState.Detached;

        var dish = _dbContext.Dishes.Find(adaptedDish.Id);
        dish!.Image.Should().BeNull();
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
