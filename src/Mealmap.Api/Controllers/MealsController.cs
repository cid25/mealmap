using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.Exceptions;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.Swagger;
using Mealmap.Domain.Common;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly ILogger<MealsController> _logger;
    private readonly MealFactory _factory;
    private readonly IMealRepository _repository;
    private readonly IMealService _mealService;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly IRequestContext _context;

    public MealsController(
        ILogger<MealsController> logger,
        MealFactory factory,
        IMealRepository mealRepository,
        IMealService mealService,
        IOutputMapper<MealDTO, Meal> outputMapper,
        IRequestContext context)
    {
        _logger = logger;
        _factory = factory;
        _repository = mealRepository;
        _mealService = mealService;
        _outputMapper = outputMapper;
        _context = context;
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
        var meals = _repository.GetAll(fromDate, toDate);

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
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public ActionResult<MealDTO> GetMeal([FromRoute] Guid id)
    {
        Meal? meal = _repository.GetSingleById(id);

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
    [SwaggerRequestExample(typeof(MealDTO), typeof(MealRequestExampleWithoutIdAndEtag))]
    [SwaggerResponseExample(201, typeof(MealResponseExampleWithIdAndEtag))]
    public ActionResult<MealDTO> PostMeal([FromBody] MealDTO dto)
    {
        if (dto.Id != Guid.Empty && dto.Id != null)
        {
            throw new ValidationException("Id not allowed as part of request.");
        }

        var meal = _factory.CreateMealWith(dto.DiningDate);

        try
        {
            SetCoursesFromDataTransferObject(meal, dto);
            _repository.Add(meal);
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
    /// Updates a specific meal.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <response code="200">Meal Updated</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Meal Not Found</response>
    /// <response code="412">ETag Doesn't Match</response>
    /// <response code="428">Update Requires ETag</response>
    [HttpPut("{id}", Name = nameof(PutMeal))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(void), StatusCodes.Status428PreconditionRequired)]
    [SwaggerRequestExample(typeof(MealDTO), typeof(MealRequestExampleWithIdWithoutEtag))]
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public ActionResult<MealDTO> PutMeal([FromRoute] Guid id, [FromBody] MealDTO dto)
    {
        if (String.IsNullOrEmpty(_context.IfMatchHeader))
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        if (dto.Id == null || id != dto.Id)
            return BadRequest("Field id is mandatory and resource must match route.");

        if (dto.Id == Guid.Empty)
            return BadRequest("Field id cannot be empty.");

        var meal = _repository.GetSingleById(id);

        if (meal == null)
            return NotFound($"Meal with id does not exist.");

        try
        {
            UpdateMealFromDataTransferObject(meal, dto);
        }
        catch (DomainValidationException)
        {
            return BadRequest("Dish with id does not exist.");
        }

        try
        {
            _repository.Update(meal);
            _logger.LogInformation("Updated dish with id {Id}", meal.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
        }

        return _outputMapper.FromEntity(meal);
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
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public ActionResult<MealDTO> DeleteMeal([FromRoute] Guid id)
    {
        var meal = _repository.GetSingleById(id);

        if (meal == null)
            return NotFound($"Meal with id does not exist");

        _repository.Remove(meal);

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

    private void UpdateMealFromDataTransferObject(Meal meal, MealDTO dto)
    {
        if (_context.IfMatchHeader == null || _context.IfMatchHeader == String.Empty)
            throw new ValidationException("The If-Match header must be set.");

        meal.Version.Set(_context.IfMatchHeader);

        meal.DiningDate = dto.DiningDate;

        meal.RemoveAllCourses();
        if (dto.Courses != null)
            foreach (var course in dto.Courses)
                _mealService.AddCourseToMeal(meal, course.Index, course.MainCourse, course.DishId);
    }
}
