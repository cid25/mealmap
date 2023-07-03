using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using AutoMapper;


namespace Mealmap.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MealsController : ControllerBase
{
    private readonly IMealRepository _repository;
    private readonly IMapper _mapper;

    public MealsController(IMealRepository repository, IMapper mapper)
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

        var mealDto = _mapper.Map<MealDto>(meal);

        return mealDto;
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
