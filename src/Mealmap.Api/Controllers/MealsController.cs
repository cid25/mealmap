using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;


namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly IMealRepository _mealRepository;
    private readonly IDishRepository _dishRepository;
    private readonly MealMapper _mapper;

    public MealsController(IMealRepository mealRepository, IDishRepository dishRepository, MealMapper mapper)
    {
        _mealRepository = mealRepository;
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<MealDTO>> GetMeals()
    {
        var meals = _mealRepository.GetAll();
        var mealDTOs = _mapper.MapFromEntities(meals);

        return mealDTOs;
    }


    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MealDTO> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _mealRepository.GetById(id);
               
        if (meal == null)
            return NotFound();

        return _mapper.MapFromEntity(meal);
    }

    [HttpPost(Name = nameof(PostMeal))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO mealDto)
    {
        if (mealDto.Id != null)
            return BadRequest();

        mealDto = mealDto with { Id = Guid.NewGuid() };
        var meal = _mapper.MapFromDTO(mealDto);

        _mealRepository.Create(meal);

        var mealCreated = _mapper.MapFromEntity(meal);

        return CreatedAtAction(nameof(GetMeal), new { id = mealCreated.Id }, mealCreated);
    }
}
