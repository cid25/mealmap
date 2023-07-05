using Microsoft.AspNetCore.Mvc;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using AutoMapper;


namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public ActionResult<IEnumerable<MealDTO>> GetMeals()
    {
        var meals = _repository.GetAll();
        var mealDtos = _mapper.Map<IEnumerable<Meal>, List<MealDTO>>(meals);

        return mealDtos;
    }


    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    public ActionResult<MealDTO> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _repository.GetById(id);
               
        if (meal == null)
            return NotFound();

        return _mapper.Map<MealDTO>(meal);
    }

    [HttpPost(Name = nameof(PostMeal))]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO mealDto)
    {
        if (mealDto.Id != null)
            return BadRequest();

        mealDto = mealDto with { Id = Guid.NewGuid() };
        var meal = _mapper.Map<Meal>(mealDto);

        _repository.Create(meal);

        var mealCreated = _mapper.Map<MealDTO>(meal);

        return mealCreated;
    }
}
