using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.Swagger;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.MealAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[RequiredScope("access")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class MealsController : ControllerBase
{
    private readonly ILogger<MealsController> _logger;
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;

    public MealsController(
        ILogger<MealsController> logger,
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        IRequestContext context,
        IMediator mediator)
    {
        _logger = logger;
        _repository = mealRepository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _context = context;
        _mediator = mediator;
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
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [SwaggerResponseExample(200, typeof(MealsResponseExample))]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(MealDTO), typeof(MealRequestExampleWithoutIdAndEtag))]
    [SwaggerResponseExample(201, typeof(MealResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<MealDTO>> PostMeal([FromBody] MealDTO dto)
    {
        var command = new CreateMealCommand(dto);
        var result = await _mediator.Send(command);

        if (!result.Succeeded && result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest(String.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));

        return CreatedAtAction(nameof(GetMeal), new { id = result.Result!.Id }, result.Result);
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [SwaggerRequestExample(typeof(MealDTO), typeof(MealRequestExampleWithIdWithoutEtag))]
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<MealDTO>> PutMeal([FromRoute] Guid id, [FromBody] MealDTO dto)
    {
        if (String.IsNullOrEmpty(_context.IfMatchHeader))
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        var command = new UpdateMealCommand(id, _context.IfMatchHeader, dto);
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.VersionMismatch))
                return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);

            if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound))
                return NotFound(String.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotFound).Select(er => er.Message)));

            if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
                return BadRequest(String.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));
        }

        return result.Result!;
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<MealDTO>> DeleteMeal([FromRoute] Guid id)
    {
        var meal = _repository.GetSingleById(id);

        if (meal == null)
            return NotFound($"Meal with id does not exist");

        _repository.Remove(meal);
        await _unitOfWork.SaveTransactionAsync();

        var dto = _outputMapper.FromEntity(meal);
        return Ok(dto);
    }
}
