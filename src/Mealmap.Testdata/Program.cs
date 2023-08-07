using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
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
    dbContext.AddRange(dishes);

    var meals = GenerateMeals(
        new MealService(new SqlDishRepository(dbContext)),
        dishes);
    dbContext.AddRange(meals);

    dbContext.SaveChanges();
}

static Dish[] GenerateDishes()
{
    Dish[] dishes = new Dish[2];

    dishes[0] = new Dish("Krabby Patty", "The fishiest burger in town.", 2);
    dishes[0].AddIngredient(4, "Slice", "Old bread");
    dishes[0].AddIngredient(2, "Piece", "Unidentifiable meat");
    dishes[0].AddIngredient(20, "Mililiter", "Fishy sauce");

    dishes[1] = new Dish("KSailors Surprise", "The darkest, wettest dream of every boatsman.", 4);
    dishes[1].AddIngredient(800, "Mililiter", "Seawater");
    dishes[1].AddIngredient(6, "Piece", "Sea cucumber");
    dishes[1].AddIngredient(8, "Piece", "Crab meat");
    dishes[1].AddIngredient(1, "Pinch", "Salt");

    return dishes;
}

static Meal[] GenerateMeals(MealService service, Dish[] dishes)
{
    Meal[] meals = new Meal[35];

    var today = DateTime.Today;
    DateOnly startOfWeek = DateOnly.FromDateTime(today).AddDays(-1 * (int)today.DayOfWeek + 1);

    for (int i = 0; i < 35; i++)
    {
        meals[i] = new Meal(startOfWeek.AddDays(-14).AddDays(i));
        service.AddCourseToMeal(meals[i], index: 1, mainCourse: true, dishId: dishes[0].Id);
        service.AddCourseToMeal(meals[i], index: 2, mainCourse: false, dishId: dishes[1].Id);

    }

    return meals;
}
