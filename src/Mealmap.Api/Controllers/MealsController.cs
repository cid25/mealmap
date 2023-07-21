using Mealmap.Api.DataTransfer;
using Mealmap.Api.Swashbuckle;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

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
    /// <response code="204">No Meals Found</response>
    /// <param name="fromDate">Date from which to include meals.</param>
    /// <param name="toDate">Date until which to include meals.</param>
    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<MealDTO>> GetMeals([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
    {
        var meals = _mealRepository.GetAll(fromDate, toDate);

        if (!meals.Any())
            return NoContent();

        var mealDTOs = _mapper.MapFromEntities(meals);

        return mealDTOs;
    }

    /// <summary>
    /// Retrieves a specific meal.
    /// </summary>
    /// <param name="id">The id of the meal to retrieve.</param>
    /// <response code="200">Meal Returned</response>
    /// <response code="404">Meal Not Found</response>
    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public ActionResult<MealDTO> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _mealRepository.GetSingle(id);

        if (meal == null)
        {
            _logger.LogInformation("Client attempt to retrieve non-existing meal with Id {Id]", id);
            return NotFound($"Meal with id {id} doesn't exist");
        }

        return _mapper.MapFromEntity(meal);
    }

    /// <summary>
    /// Creates a meal.
    /// </summary>
    /// <response code="201">Meal Created</response>
    /// <response code="400">Bad Request</response>
    [HttpPost(Name = nameof(PostMeal))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(MealDTO), typeof(MealPostRequestExample))]
    [SwaggerResponseExample(201, typeof(MealPostResponseExample))]
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO mealDTO)
    {
        Meal meal;

        try
        {
            meal = _mapper.MapFromDTO(mealDTO);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        _mealRepository.Add(meal);
        _logger.LogInformation("Created meal with id {Id}", meal.Id);

        var mealCreated = _mapper.MapFromEntity(meal);
        return CreatedAtAction(nameof(GetMeal), new { id = mealCreated.Id }, mealCreated);
    }

    /// <summary>
    /// Deletes a specific meal.
    /// </summary>
    /// <param name="id">The id of the meal to delete.</param>
    /// <response code="200">Meal Deleted</response>
    /// <response code="404">Meal Not Found</response>
    [HttpDelete("{id}", Name = nameof(DeleteMeal))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public ActionResult<MealDTO> DeleteMeal([FromRoute] Guid id)
    {
        var meal = _mealRepository.GetSingle(id);

        if (meal == null)
            return NotFound($"Meal with id {id} doesn't exist");

        _mealRepository.Remove(meal);

        var dto = _mapper.MapFromEntity(meal);
        return Ok(dto);
    }
}
