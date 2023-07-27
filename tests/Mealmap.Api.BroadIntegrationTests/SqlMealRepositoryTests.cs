using FluentAssertions;
using Mealmap.DataAccess;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.BroadIntegrationTests;

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
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
            .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
            .Options;
        _dbContext = new MealmapDbContext(dbOptions);
        _repository = new SqlMealRepository(_dbContext);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        seedData();
    }

    private void seedData()
    {
        _dishes = new Dish[1];
        _dishes[0] = new Dish("Sailors Surprise") { Id = Guid.NewGuid() };
        _dbContext.Add(_dishes[0]);

        _meals = new Meal[4];
        for (int day = 1; day <= 4; day++)
        {
            _meals[day - 1] = new Meal(id: Guid.NewGuid(), diningDate: new DateOnly(2020, 1, day));
            _meals[day - 1].AddCourse(1, true, _dishes[0].Id);
        }
        _dbContext.Meals.AddRange(_meals);

        _dbContext.SaveChanges();
        Helpers.DetachAllEntities(_dbContext);
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
        result!.Courses.Should().HaveCount(1);
    }

    [Fact]
    public void Add_WhenMealValid_CreatesEntry()
    {
        var SomeGuid = Guid.NewGuid();
        Meal meal = new(id: SomeGuid, diningDate: new DateOnly(2020, 12, 31));

        _repository.Add(meal);

        _dbContext.Meals.First(x => x.Id == SomeGuid).Should().NotBeNull();
    }

    //[Fact]
    //public void Add_WhenCourseReferencesNonExistingDish_ThrowsException()
    //{
    //    var nonExistingDishGuid = Guid.NewGuid();
    //    var meal = new Meal(id: Guid.NewGuid(), diningDate: DateOnly.FromDateTime(DateTime.Now));
    //    meal.AddCourse


    //    meal.SetOrderOfCourses(new List<Course>() {
    //        new Course(1, true, nonExistingDishGuid)
    //    });

    //    Action act = () => _repository.Add(meal);

    //    act.Should().Throw<DomainValidationException>();
    //}

    [Fact]
    public void Remove_WhenGivenUntrackedEntity_RemovesEntry()
    {
        var expectedCount = _meals!.Length;
        var meal = _meals![0];

        _repository.Remove(meal);

        _dbContext.Meals.Count().Should().Be(expectedCount - 1);
    }

    [Fact]
    public void Remove_WhenGivenTrackedEntity_RemovesEntry()
    {
        var expectedCount = _meals!.Length;
        var meal = _dbContext.Meals.Find(_meals[0].Id);

        _repository.Remove(meal!);

        _dbContext.Meals.Count().Should().Be(expectedCount - 1);
    }
}
