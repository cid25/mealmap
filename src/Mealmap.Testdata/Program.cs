using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure;
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
    var dishes = GenerateDishes(new DishService());
    dbContext.Dishes.AddRange(dishes);

    var meals = GenerateMeals(
        new MealService(
            new SqlDishRepository(dbContext)),
        dishes);
    dbContext.Meals.AddRange(meals);

    dbContext.SaveChanges();
}

static Dish[] GenerateDishes(DishService service)
{
    Dish[] dishes = new Dish[2];

    dishes[0] = service.Create("Krabby Patty", "The fishiest burger in town.", 2);
    service.AddIngredient(dishes[0], 4, "Slice", "Old bread");
    service.AddIngredient(dishes[0], 2, "Piece", "Unidentifiable meat");
    service.AddIngredient(dishes[0], 20, "Mililiter", "Fishy sauce");

    dishes[1] = service.Create("KSailors Surprise", "The darkest, wettest dream of every boatsman.", 4);
    service.AddIngredient(dishes[1], 800, "Mililiter", "Seawater");
    service.AddIngredient(dishes[1], 6, "Piece", "Sea cucumber");
    service.AddIngredient(dishes[1], 8, "Piece", "Crab meat");
    service.AddIngredient(dishes[1], 1, "Pinch", "Salt");

    return dishes;
}

static Meal[] GenerateMeals(MealService service, Dish[] dishes)
{
    Meal[] meals = new Meal[35];

    var today = DateTime.Today;
    DateOnly startOfWeek = DateOnly.FromDateTime(today).AddDays(-1 * (int)today.DayOfWeek + 1);

    for (int i = 0; i < 35; i++)
    {
        Meal meal = service.CreateMeal(startOfWeek.AddDays(-14).AddDays(i));
        service.AddCourseToMeal(meal, index: 1, mainCourse: true, dishId: dishes[0].Id);
        service.AddCourseToMeal(meal, index: 2, mainCourse: false, dishId: dishes[1].Id);
        meals[i] = meal;
    }

    return meals;
}
