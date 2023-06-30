using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;


namespace Mealmap.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MealController : ControllerBase
{

    [HttpGet(Name = "GetMeal")]
    public ActionResult<MealDto> Get()
    {
        return new MealDto(name: "Cheeseburger");
    }
}
