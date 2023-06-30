using Microsoft.AspNetCore.Mvc;
using Mealmap.Model;


namespace Mealmap.Controllers;

[ApiController]
[Route("[controller]")]
public class MealController : ControllerBase
{

    [HttpGet(Name = "GetMeal")]
    public ActionResult<Meal> Get()
    {
        return new Meal(name: "Cheeseburger");
    }
}
