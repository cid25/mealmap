using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.Exceptions;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.Swagger;
using Mealmap.Domain.Exceptions;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly ILogger<MealsController> _logger;
    private readonly IMealRepository _mealRepository;
    private readonly IMealService _mealService;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;

    public MealsController(
        ILogger<MealsController> logger,
        IMealRepository mealRepository,
        IMealService mealService,
        IOutputMapper<MealDTO, Meal> outputMapper)
    {
        _logger = logger;
        _mealRepository = mealRepository;
        _mealService = mealService;
        _outputMapper = outputMapper;
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

        var dataTransferObjects = _outputMapper.FromEntities(meals);

        return Ok(dataTransferObjects);
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
        Meal? meal = _mealRepository.GetSingleById(id);

        if (meal == null)
        {
            _logger.LogInformation("Client attempt to retrieve non-existing meal with Id {Id]", id);
            return NotFound($"Meal with id does not exist.");
        }

        return _outputMapper.FromEntity(meal);
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
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO dto)
    {
        if (dto.Id != Guid.Empty && dto.Id != null)
        {
            throw new ValidationException("Id not allowed as part of request.");
        }

        var meal = _mealService.CreateMeal(dto.DiningDate);

        try
        {
            SetCoursesFromDataTransferObject(meal, dto);
            _mealRepository.Add(meal);
        }
        catch (Exception ex)
            when (ex is ConcurrentUpdateException || ex is DomainValidationException)
        {
            _logger.LogInformation(ex.Message);
            return BadRequest(ex.Message);
        }

        var mealCreated = _outputMapper.FromEntity(meal);
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
        var meal = _mealRepository.GetSingleById(id);

        if (meal == null)
            return NotFound($"Meal with id does not exist");

        _mealRepository.Remove(meal);

        var dto = _outputMapper.FromEntity(meal);
        return Ok(dto);
    }

    /// <exception cref="DomainValidationException"/>
    private void SetCoursesFromDataTransferObject(Meal meal, MealDTO dto)
    {
        if (dto.Courses != null)
        {
            foreach (var course in dto.Courses)
            {
                _mealService.AddCourseToMeal(meal, course.Index, course.MainCourse, course.DishId);
            }
        }
    }
}
