using Bogus;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mealmap.Api.IntegrationTests;

internal static class DatabaseSeeder
{
    private static readonly Faker _faker = new();

    public static List<Dish> Dishes = [];
    public static List<Meal> Meals = [];

    /// <summary>
    ///  Injects the common set of seed data into an external SQL Server database.
    /// </summary>
    /// <returns>The dbContext for usage in tests.</returns>
    public static MealmapDbContext Init(bool withData = true)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("settings.json")
            .Build();
        var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
            .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
            .Options;
        var context = new MealmapDbContext(dbOptions);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (withData) DatabaseSeeder.InjectSeedData(context);

        return context;
    }

    public static Dish GetRandomDish()
    {
        var index = _faker.Random.Number(Dishes.Count - 1);
        return Dishes.ElementAt(index);
    }

    public static Meal GetRandomMeal()
    {
        var index = _faker.Random.Number(Dishes.Count - 1);
        return Meals.ElementAt(index);
    }

    private static void InjectSeedData(MealmapDbContext context)
    {
        GenerateData();
        context.AddRange(DatabaseSeeder.Dishes);
        context.AddRange(DatabaseSeeder.Meals);
        context.SaveChanges();

        context.ChangeTracker.Clear();
    }

    private static void GenerateData()
    {
        Randomizer.Seed = new Random(1337);

        GenerateDishes();
        GenerateMeals();
    }

    private static void GenerateDishes()
    {
        for (var i = 0; i < 4; i++)
        {
            Dish dish = new(name: _faker.Commerce.Product())
            {
                Description = _faker.Lorem.Sentence(5),
                Servings = _faker.Random.Number(1, 10),
                Instructions = _faker.Lorem.Sentence(60)
            };

            if (i % 2 == 0) dish.SetImage(new byte[1], "image/png");

            for (var j = 0; j < _faker.Random.Number(0, 3); j++)
            {
                dish.AddIngredient(_faker.Random.Decimal(0, 20), _faker.Hacker.Noun(), _faker.Commerce.Product());
            }

            Dishes.Add(dish);
        }
    }

    private static void GenerateMeals()
    {
        var startDate = new DateOnly(2020, 1, 1);

        for (var i = 0; i < 10; i++)
        {
            Meal meal = new(startDate.AddDays(i));

            for (var j = 1; j < _faker.Random.Number(4); j++)
                meal.AddCourse(index: j, mainCourse: j == 0, attendees: _faker.Random.Number(1, 6), dishId: GetRandomDish().Id);

            Meals.Add(meal);
        }

    }
}
