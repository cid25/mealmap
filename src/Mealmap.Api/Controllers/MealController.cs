using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using AutoMapper;


namespace Mealmap.Api.Controllers;

[ApiController]
[Route("meals")]
public class MealController : ControllerBase
{
    private readonly IMealRepository _repository;
    private readonly IMapper _mapper;

    public MealController(IMealRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    public ActionResult<IEnumerable<MealDto>> GetMeals()
    {
        return new List<MealDto>();
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

    [HttpPost(Name = nameof(PostMeal))]
    [Consumes("application/json")]
    public ActionResult PostMeal([FromBody] MealDto mealDto)
    {
        var meal = _mapper.Map<Meal>(mealDto);
        _repository.Create(meal);

        return Ok();
    }
}
