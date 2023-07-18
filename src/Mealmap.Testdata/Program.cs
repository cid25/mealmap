using Mealmap.Api.DataAccess;
using Mealmap.Model;
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


void InjectTestData(MealmapDbContext dbContext)
{
    InjectDishes(dbContext);
    InjectMeals(dbContext);
}

void InjectDishes(MealmapDbContext dbContext)
{
    dbContext.Dishes.Add(new Dish("Krabby Patty")
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
    dbContext.Dishes.Add(new Dish("Sailors Surprise")
    {
        Id = new Guid("00000000-0000-0000-0000-000000000002"),
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

void InjectMeals(MealmapDbContext dbContext)
{
    var today = DateTime.Today;
    DateOnly startOfWeek = DateOnly.FromDateTime(today).AddDays(-1 * (int)today.DayOfWeek + 1);

    for (int i = 0; i < 7; i++)
    {
        dbContext.Meals.Add(new Meal()
        {
            DiningDate = startOfWeek.AddDays(i),
            Dish = dbContext.Dishes.Find(Guid.Parse("00000000-0000-0000-0000-00000000000" + (i % 2 + 1).ToString())),
        });
    }

    dbContext.SaveChanges();
}
