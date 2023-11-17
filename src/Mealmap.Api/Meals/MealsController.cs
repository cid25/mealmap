using Mealmap.Api.Dishes;
using Mealmap.Api.Shared;
using Mealmap.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Meals;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[RequiredScope("access")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class MealsController(IRequestContext context) : ControllerBase
{
    /// <summary>
    /// Lists meals.
    /// </summary>
    /// <response code="200">Meals Returned</response>
    /// <response code="204">No Meals Found</response>
    /// <param name="fromDate">Date from which to include meals.</param>
    /// <param name="toDate">Date until which to include meals.</param>
    /// <param name="responder"></param>
    [HttpGet(Name = nameof(GetMeals))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [SwaggerResponseExample(200, typeof(MealsResponseExample))]
    public async Task<ActionResult<IEnumerable<MealDTO>>> GetMeals(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromServices] IQueryResponder<MealsQuery, IEnumerable<MealDTO>> responder
    )
    {
        var query = new MealsQuery(fromDate, toDate);
        var result = await responder.RespondTo(query);

        if (!result.Any())
            return NoContent();

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific meal.
    /// </summary>
    /// <param name="id">The id of the meal to retrieve.</param>
    /// <param name="responder"></param>
    /// <response code="200">Meal Returned</response>
    /// <response code="404">Meal Not Found</response>
    [HttpGet("{id}", Name = nameof(GetMeal))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<MealDTO>> GetMeal(
        [FromRoute] Guid id,
        [FromServices] IQueryResponder<MealQuery, MealDTO?> responder
    )
    {
        var query = new MealQuery(id);
        var result = await responder.RespondTo(query);

        if (result == null) return NotFound($"Meal with id {id} does not exist.");

        return result;
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
    public async Task<ActionResult<MealDTO>> PostMeal(
        [FromBody] MealDTO dto,
        [FromServices] ICommandProcessor<CreateMealCommand, MealDTO> processor
    )
    {
        var command = new CreateMealCommand(dto);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest(string.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));

        return CreatedAtAction(nameof(GetMeal), new { id = result.Result!.Id }, result.Result);
    }

    /// <summary>
    /// Updates a specific meal.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="processor"></param>
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
    public async Task<ActionResult<MealDTO>> PutMeal(
        [FromRoute] Guid id,
        [FromBody] MealDTO dto,
        [FromServices] ICommandProcessor<UpdateMealCommand, MealDTO> processor
    )
    {
        if (string.IsNullOrEmpty(context.IfMatchHeader))
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        var command = new UpdateMealCommand(id, context.IfMatchHeader, dto);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.VersionMismatch))
            return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound))
            return NotFound(string.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotFound).Select(er => er.Message)));

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest(string.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));

        return result.Result!;
    }

    /// <summary>
    /// Deletes a specific meal.
    /// </summary>
    /// <param name="id">The id of the meal to delete.</param>
    /// <param name="processor"></param>
    /// <response code="200">Meal Deleted</response>
    /// <response code="404">Meal Not Found</response>
    [HttpDelete("{id}", Name = nameof(DeleteMeal))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(MealResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<MealDTO>> DeleteMeal(
        [FromRoute] Guid id,
        [FromServices] ICommandProcessor<DeleteMealCommand, MealDTO> processor
    )
    {
        var command = new DeleteMealCommand(id);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound))
            return NotFound($"Meal with id does not exist");

        return Ok(result.Result);
    }
}
