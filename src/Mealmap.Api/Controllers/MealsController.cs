using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;


namespace Mealmap.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly ILogger<MealsController> _logger;
    private readonly IMealRepository _mealRepository;
    private readonly IDishRepository _dishRepository;
    private readonly MealMapper _mapper;

    public MealsController(
        ILogger<MealsController> logger,
        IMealRepository mealRepository, 
        IDishRepository dishRepository, 
        MealMapper mapper)
    {
        _logger = logger;
        _mealRepository = mealRepository;
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Lists meals.
    /// </summary>
    /// <response code="200">Meals Returned</response>
    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<MealDTO>> GetMeals()
    {
        var meals = _mealRepository.GetAll();
        var mealDTOs = _mapper.MapFromEntities(meals);

        return mealDTOs;
    }

    /// <summary>
    /// Retrieves a specific meal.
    /// </summary>
    /// <param name="id">The id of the meal.</param>
    /// <response code="200">Meal Returned</response>
    /// <response code="404">Meal Not Found</response>
    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MealDTO> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _mealRepository.GetById(id);

        if (meal == null)
        {
            _logger.LogInformation("Attempt to retrieve non-existing meal");
            return NotFound();
        }

        return _mapper.MapFromEntity(meal);
    }

    /// <summary>
    /// Creates a meal.
    /// </summary>
    /// <param name="mealDTO"></param>
    /// <response code="201">Meal Created</response>
    [HttpPost(Name = nameof(PostMeal))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO mealDTO)
    {
        mealDTO = mealDTO with { Id = Guid.NewGuid() };
        var meal = _mapper.MapFromDTO(mealDTO);

        _mealRepository.Create(meal);
        _logger.LogInformation("Meal with id {guid} created", meal.Id);

        var mealCreated = _mapper.MapFromEntity(meal);

        return CreatedAtAction(nameof(GetMeal), new { id = mealCreated.Id }, mealCreated);
    }
}
