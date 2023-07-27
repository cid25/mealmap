using Mealmap.DataAccess;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var dbOptions = new DbContextOptionsBuilder<MealmapDbContext>()
    .UseSqlServer(configuration.GetConnectionString("MealmapDb"))
    .Options;
MealmapDbContext dbContext = new MealmapDbContext(dbOptions);

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();

InjectTestData(dbContext);


static void InjectTestData(MealmapDbContext dbContext)
{
    var dishes = GenerateDishes();
    dbContext.Dishes.AddRange(dishes);

    var meals = GenerateMeals(
        new MealService(
            new SqlDishRepository(dbContext)),
        dishes);
    dbContext.Meals.AddRange(meals);

    dbContext.SaveChanges();
}

static Dish[] GenerateDishes()
{
    Dish[] result = new Dish[2];

    result[0] = new Dish("Krabby Patty")
    {
        Id = Guid.NewGuid(),
        Description = "The fishiest burger in town.",
        Servings = 2,
        Ingredients = new(){
            new Ingredient(4, new UnitOfMeasurement(UnitOfMeasurementCodes.Slice), "Old bread"),
            new Ingredient(2, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Unidentifiable meat"),
            new Ingredient(20, new UnitOfMeasurement(UnitOfMeasurementCodes.Mililiter), "Fishy sauce"),
        }
    };
    result[1] = (new Dish("Sailors Surprise")
    {
        Id = Guid.NewGuid(),
        Description = "The darkest, wettest dream of every boatsman.",
        Servings = 4,
        Ingredients = new(){
            new Ingredient(800, new UnitOfMeasurement(UnitOfMeasurementCodes.Mililiter), "Seawater"),
            new Ingredient(6, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Sea cucumber"),
            new Ingredient(8, new UnitOfMeasurement(UnitOfMeasurementCodes.Piece), "Crab meat"),
            new Ingredient(1, new UnitOfMeasurement(UnitOfMeasurementCodes.Pinch), "Salt"),
        }
    });

    return result;
}

static Meal[] GenerateMeals(MealService service, Dish[] dishes)
{
    Meal[] result = new Meal[35];

    var today = DateTime.Today;
    DateOnly startOfWeek = DateOnly.FromDateTime(today).AddDays(-1 * (int)today.DayOfWeek + 1);

    for (int i = 0; i < 35; i++)
    {
        Meal meal = service.CreateMeal(startOfWeek.AddDays(-14).AddDays(i));
        service.AddCourseToMeal(meal, index: 1, mainCourse: true, dishId: dishes[0].Id);
        service.AddCourseToMeal(meal, index: 2, mainCourse: false, dishId: dishes[1].Id);
        result[i] = meal;
    }

    return result;
}
