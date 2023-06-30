using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MealController : ControllerBase
{
    private readonly IMealRepository _repository;

    public MealController(IMealRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}", Name = "GetMeal")]
    public ActionResult<MealDto> Get([FromRoute] Guid id)
    {
        Meal? meal = _repository.GetById(id);
               
        if (meal == null)
        { 
            return NotFound();
        }
        
        return new MealDto(meal.Id, meal.Name);
    }
}
