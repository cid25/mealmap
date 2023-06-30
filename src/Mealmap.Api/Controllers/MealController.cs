using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Controllers;

[ApiController]
[Route("[controller]")]
public class MealController : ControllerBase
{

    [HttpGet(Name = "GetMeal")]
    public IActionResult Get()
    {
        return Ok();
    }
}
