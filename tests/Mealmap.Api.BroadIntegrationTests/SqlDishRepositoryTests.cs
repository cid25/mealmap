using FluentAssertions;
using Mealmap.Api.DataAccess;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.BroadIntegrationTests
{
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
            seedData(_dbContext);

            _repository = new SqlDishRepository(_dbContext);
        }

        private void seedData(MealmapDbContext dbContext)
        {
            dbContext.Dishes.Add(new Dish("Krabby Patty") { 
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
            dbContext.Dishes.Add(new Dish("Sailors Surprise") { 
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
            dbContext.SaveChanges();
        }


        [Fact]
        public void GetAll_ReturnsAllDishes()
        {
            var expectedCount = _dbContext.Dishes.Count();
            
            var result = _repository.GetAll();

            result.Should().NotBeEmpty().And.HaveCount(expectedCount);
        }

        [Fact]
        public void GetById_WhenIdNonExisting_ReturnsNull()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _repository.GetById(new Guid(nonExistingGuid));

            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WhenIdExists_ReturnsDish()
        {
            const string existingGuid = "00000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
            result!.Name.Should().Be("Krabby Patty");
        }

        [Fact]
        public void Create_WhenDishValid_CreatesEntry()
        {
            const string someDishName = "Salty Sea Dog";
            Guid someGuid = Guid.NewGuid();
            Dish dish = new(someDishName) { Id = someGuid };

            _repository.Create(dish);

            _dbContext.Dishes.First(x => x.Id == someGuid).Should().NotBeNull();
        }

        [Fact]
        public void Create_WhenDishHasIngredients_CreatesIngredients()
        {
            const string someDishName = "Salty Sea Dog";
            Guid someGuid = Guid.NewGuid();
            Dish dish = new(someDishName) { Id = someGuid };
            dish.AddIngredient(1, "Kilogram", "Sausages");
            dish.AddIngredient(0.5m, "Liter", "Ketchup");
            dish.AddIngredient(0.3m, "Liter", "Mustard");

            _repository.Create(dish);

            _dbContext.Dishes.First(x => x.Id == someGuid).Ingredients.Should().HaveCount(3);
        }

        [Fact] 
        public void Update_WhenGivenDisconnectedDish_UpdatesEntry()
        {
            const string someDishName = "Salty Sea Dog";
            Guid guid = Guid.NewGuid();
            Dish initialDish = new(someDishName) { Id = guid };
            _dbContext.Dishes.Add(initialDish);
            _dbContext.SaveChanges();
            _dbContext.Entry(initialDish).State = EntityState.Detached;

            const string anotherDishName = "Tuna Supreme";
            var newDisconnectedDish = new Dish(anotherDishName) { Id = guid };
            _repository.Update(newDisconnectedDish);

            _dbContext.Dishes.First(d => d.Id == guid).Name.Should().Be(anotherDishName);
        }

        [Fact]
        public void Update_WhenIngredientAdded_AddsIngredient()
        {
            var originalDish = _dbContext.Dishes.First();
            var originalIngredientCount = originalDish.Ingredients!.Count();
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

            _dbContext.Dishes.First(d => d.Id == originalDish.Id).Ingredients.Should().HaveCount(originalIngredientCount + 1);
        }

        [Fact]
        public void Update_WhenIngredientRemoved_RemovesIngredient()
        {
            var originalDish = _dbContext.Dishes.First(d => d.Ingredients != null && d.Ingredients.Count > 2);
            var originalIngredientCount = originalDish.Ingredients!.Count();
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

            _dbContext.Dishes.First(d => d.Id == originalDish.Id).Ingredients.Should().HaveCount(originalIngredientCount - 1);
        }
    }
}
