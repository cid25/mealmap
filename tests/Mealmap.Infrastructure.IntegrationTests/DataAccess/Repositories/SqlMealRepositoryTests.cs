using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Mealmap.Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Infrastructure.IntegrationTests.DataAccess.Repositories;

[Collection("InSequence")]
[Trait("Target", "Database")]
public class SqlMealRepositoryTests
{
    private readonly MealmapDbContext _dbContext;
    private readonly SqlMealRepository _repository;
    private Dish[]? _dishes;
    private Meal[]? _meals;

    public SqlMealRepositoryTests()
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
            .Options;
        _dbContext = new MealmapDbContext(dbOptions);
        _repository = new SqlMealRepository(_dbContext);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        seedData();
        _dbContext.ChangeTracker.Clear();
    }

    private void seedData()
    {
        _dishes = new Dish[1];
        _dishes[0] = new("Sailors Surprise", null, 1);
        _dbContext.Add(_dishes[0]);

        _meals = new Meal[4];
        for (var day = 1; day <= 4; day++)
        {
            _meals[day - 1] = new(diningDate: new DateOnly(2020, 1, day));
            _meals[day - 1].AddCourse(index: 1, mainCourse: true, attendees: 2, _dishes[0].Id);
        }
        _dbContext.AddRange(_meals);

        _dbContext.SaveChanges();
    }


    [Fact]
    public void GetAll_WithoutArguments_ReturnsAllMeals()
    {
        var expectedCount = _meals!.Length;

        var result = _repository.GetAll();

        result.Should().NotBeEmpty().And.HaveCount(expectedCount);
    }

    [Fact]
    public void GetAll_WhenFilteringFromDate_ReturnsProperSet()
    {
        DateOnly fromDate = new(2020, 1, 2);

        var result = _repository.GetAll(fromDate: fromDate);

        result.Should().NotBeEmpty().And.HaveCount(3);
        result.Should().NotContain(m => m.Id == _meals![0].Id);
    }

    [Fact]
    public void GetAll_WhenFilteringToDate_ReturnsProperSet()
    {
        DateOnly toDate = new(2020, 1, 3);

        var result = _repository.GetAll(toDate: toDate);

        result.Should().NotBeEmpty().And.HaveCount(3);
        result.Should().NotContain(m => m.Id == _meals![3].Id);
    }

    [Fact]
    public void GetAll_WhenFilteringFromAndToDate_ReturnsProperSet()
    {
        DateOnly fromDate = new(2020, 1, 2);
        DateOnly toDate = new(2020, 1, 3);

        var result = _repository.GetAll(fromDate, toDate);

        result.Should().NotBeEmpty().And.HaveCount(2);
        result.Should().NotContain(m => m.Id == _meals![0].Id);
        result.Should().NotContain(m => m.Id == _meals![3].Id);
    }

    [Fact]
    public void GetSingleById_WhenMealWithIdNonExisting_ReturnsNull()
    {
        var nonExistingGuid = Guid.NewGuid();
        var result = _repository.GetSingleById(nonExistingGuid);

        result.Should().BeNull();
    }

    [Fact]
    public void GetSingleById_WhenMealWithIdExists_ReturnsMealWithCourse()
    {
        var existingGuid = _meals![1].Id;
        var result = _repository.GetSingleById(existingGuid);

        result.Should().NotBeNull();
        result!.Courses.Should().ContainSingle();
    }

    [Fact]
    public void SaveOnAdd_WhenMealValid_CreatesEntry()
    {
        var aGuid = Guid.NewGuid();
        Meal meal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        _repository.Add(meal);
        _dbContext.SaveChanges();

        _dbContext.Find<Meal>(meal.Id).Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenMealDisconnected_ThrowsInvalidOperationException()
    {
        var initialMeal = _dbContext.Find<Meal>(_meals![0].Id);
        Meal disconnectedMeal = new(initialMeal!.Id, initialMeal.DiningDate);

        Action act = () => _repository.Update(disconnectedMeal);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SaveOnUpdate_WhenConcurrentUpdate_ThrowsException()
    {
        var meal = _dbContext.Find<Meal>(_meals![1].Id)!;
        _dbContext.Entry(meal).Property(m => m.DiningDate).IsModified = true;

        _dbContext.Database.ExecuteSqlRaw("UPDATE [mealmap].[meal] SET [DiningDate] = '2020-01-31' WHERE [Id] = '" + meal.Id + "';");
        _repository.Update(meal);
        Action act = () => _dbContext.SaveChanges();

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void SaveOnUpdate_WhenExplicitVersionNotMatchingDatabase_ThrowsException()
    {
        var meal = _dbContext.Find<Meal>(_meals![1].Id)!;
        _dbContext.Entry(meal).Property(m => m.DiningDate).IsModified = true;
        var nonMatchingVersion = "AAAA";
        meal!.Version.Set(nonMatchingVersion);

        _repository.Update(meal);
        Action act = () => _dbContext.SaveChanges();

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void SaveOnUpdate_WhenCourseAdded_AddsCourseAndUpsMealVersion()
    {
        var meal = _dbContext.Find<Meal>(_meals![0].Id)!;
        var originalCount = meal!.Courses.Count;
        var originalVersion = meal.Version;

        meal.AddCourse(index: 2, mainCourse: false, attendees: 2, _dishes![0].Id);
        _repository.Update(meal);
        _dbContext.SaveChanges();

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Meal>(meal.Id);
        result!.Courses.Should().HaveCount(originalCount + 1);
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void SaveOnUpdate_WhenIngredientsRemoved_RemovesIngredientAndUpsDishVersion()
    {
        var meal = _dbContext.Find<Meal>(_meals![0].Id)!;
        var originalVersion = meal.Version;

        meal!.RemoveAllCourses();
        _repository.Update(meal);
        _dbContext.SaveChanges();

        _dbContext.ChangeTracker.Clear();
        var result = _dbContext.Find<Meal>(meal.Id);
        result!.Courses.Should().BeEmpty();
        result.Version.AsBytes().Should().NotEqual(originalVersion.AsBytes());
    }

    [Fact]
    public void SaveOnRemove_WhenGivenUntrackedEntity_RemovesEntry()
    {
        var expectedCount = _meals!.Length;
        var meal = _meals![0];

        _repository.Remove(meal);
        _dbContext.SaveChanges();

        _repository.dbSet.Should().HaveCount(expectedCount - 1);
    }

    [Fact]
    public void SaveOnRemove_WhenGivenTrackedEntity_RemovesEntry()
    {
        var expectedCount = _meals!.Length;
        var meal = _dbContext.Find<Meal>(_meals[0].Id);

        _repository.Remove(meal!);
        _dbContext.SaveChanges();

        _repository.dbSet.Should().HaveCount(expectedCount - 1);
    }
}

