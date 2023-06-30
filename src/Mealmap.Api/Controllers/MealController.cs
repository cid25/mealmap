using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("meals")]
public class MealController : ControllerBase
{
    private readonly IMealRepository _repository;

    public MealController(IMealRepository repository)
    {
        _repository = repository;
    }

    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Meal>> GetMeals()
    {
        return new List<Meal>();
    }


    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    public ActionResult<MealDto> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _repository.GetById(id);
               
        if (meal == null)
        { 
            return NotFound();
        }
        
        return new MealDto(meal.Id, meal.Name);
    }
}
