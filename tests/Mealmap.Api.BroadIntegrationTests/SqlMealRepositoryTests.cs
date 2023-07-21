using FluentAssertions;
using Mealmap.Api.DataAccess;
using Mealmap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.BroadIntegrationTests
{
    [Collection("InSequence")]
    [Trait("Target", "Database")]
    public class SqlMealRepositoryTests
    {
        private readonly MealmapDbContext _dbContext;
        private readonly SqlMealRepository _repository;

        public SqlMealRepositoryTests()
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

            _repository = new SqlMealRepository(_dbContext);
        }

        private void seedData()
        {
            var dish = new Dish("Sailors Surprise") { Id = new Guid("00000000-0000-0000-0000-000000000001") };
            _dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000001"), DiningDate = new DateOnly(2020, 1, 1), Dish = dish });
            _dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000002"), DiningDate = new DateOnly(2020, 1, 2), Dish = dish });
            _dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000003"), DiningDate = new DateOnly(2020, 1, 3), Dish = dish });
            _dbContext.Meals.Add(new Meal() { Id = new Guid("10000000-0000-0000-0000-000000000004"), DiningDate = new DateOnly(2020, 1, 4), Dish = dish });
            _dbContext.SaveChanges();
        }


        [Fact]
        public void GetAll_WithoutArguments_ReturnsAllMeals()
        {
            var expectedCount = _dbContext.Meals.Count();

            var result = _repository.GetAll();

            result.Should().NotBeEmpty().And.HaveCount(expectedCount);
        }

        [Fact]
        public void GetAll_WhenFilteringFromDate_ReturnsProperSet()
        {
            DateOnly fromDate = new(2020, 1, 2);

            var result = _repository.GetAll(fromDate: fromDate);

            result.Should().NotBeEmpty().And.HaveCount(3);
            result.Should().NotContain(m => m.Id == new Guid("10000000-0000-0000-0000-000000000001"));
        }

        [Fact]
        public void GetAll_WhenFilteringToDate_ReturnsProperSet()
        {
            DateOnly toDate = new(2020, 1, 3);

            var result = _repository.GetAll(toDate: toDate);

            result.Should().NotBeEmpty().And.HaveCount(3);
            result.Should().NotContain(m => m.Id == new Guid("10000000-0000-0000-0000-000000000004"));
        }

        [Fact]
        public void GetAll_WhenFilteringFromAndToDate_ReturnsProperSet()
        {
            DateOnly fromDate = new(2020, 1, 2);
            DateOnly toDate = new(2020, 1, 3);

            var result = _repository.GetAll(fromDate, toDate);

            result.Should().NotBeEmpty().And.HaveCount(2);
            result.Should().NotContain(m => m.Id == new Guid("10000000-0000-0000-0000-000000000001"));
            result.Should().NotContain(m => m.Id == new Guid("10000000-0000-0000-0000-000000000004"));
        }

        [Fact]
        public void GetById_WhenMealWithIdNonExisting_ReturnsNull()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _repository.GetById(new Guid(nonExistingGuid));

            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WhenMealWithIdExists_ReturnsMealWithDish()
        {
            const string existingGuid = "10000000-0000-0000-0000-000000000001";
            var result = _repository.GetById(new Guid(existingGuid));

            result.Should().NotBeNull();
            result!.Dish.Should().NotBeNull();
        }

        [Fact]
        public void Create_WhenMealValid_CreatesEntry()
        {
            var dish = _dbContext.Dishes.First();
            var guid = Guid.NewGuid();
            Meal meal = new() { Id = guid, DiningDate = new DateOnly(2020, 12, 31), Dish = dish };

            _repository.Create(meal);

            _dbContext.Meals.First(x => x.Id == guid).Should().NotBeNull();
        }

    }
}
