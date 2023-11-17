using Mealmap.Api.Shared;
using Mealmap.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Dishes;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[RequiredScope("access")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class DishesController(IRequestContext context)
    : ControllerBase
{

    /// <summary>
    /// Lists dishes.
    /// </summary>
    /// <response code="200">Dishes Returned</response>
    /// <response code="400">Bad Request</response>
    [HttpGet(Name = nameof(GetDishes))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaginatedDTO<DishDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedDTO<DishDTO>>> GetDishes(
        [FromQuery] Guid? next,
        [FromQuery] int? limit,
        [FromQuery] string? search,
        [FromServices] IQueryResponder<DishesQuery, PaginatedDTO<DishDTO>> responder
    )
    {
        if (limit < 1) return BadRequest("Limit must be greater than 0.");

        DishesQuery query = new()
        {
            Limit = limit,
            Next = next,
            Searchterm = search == string.Empty ? null : search
        };

        var dto = await responder.RespondTo(query);

        return new OkObjectResult(dto);
    }

    /// <summary>
    /// Retrieves a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to retrieve.</param>
    /// <param name="responder"></param>
    /// <response code="200">Dish Returned</response>
    /// <response code="404">Dish Not Found</response>
    [HttpGet("{id}", Name = nameof(GetDish))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> GetDish(
        [FromRoute] Guid id,
        [FromServices] IQueryResponder<DishQuery, DishDTO?> responder
    )
    {
        var query = new DishQuery(id);
        var dto = await responder.RespondTo(query);

        if (dto == null) return NotFound();
        return dto;
    }

    /// <summary>
    /// Creates a dish.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="processor"></param>
    /// <response code="201">Dish Created</response>
    /// <response code="400">Bad Request</response>
    [HttpPost(Name = nameof(PostDish))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(DishDTO), typeof(DishRequestExampleWithoutIdAndEtag))]
    [SwaggerResponseExample(201, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> PostDish(
        [FromBody] DishDTO dto,
        [FromServices] ICommandProcessor<CreateDishCommand, DishDTO> processor
    )
    {
        var command = new CreateDishCommand(dto);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest(string.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));

        return CreatedAtAction(nameof(GetDish), new { id = result.Result!.Id }, result.Result);
    }

    /// <summary>
    /// Updates a specific dish.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    /// <param name="processor"></param>
    /// <response code="200">Dish Updated</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Dish Not Found</response>
    /// <response code="412">ETag Doesn't Match</response>
    /// <response code="428">Update Requires ETag</response>
    [HttpPut("{id}", Name = nameof(PutDish))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [SwaggerRequestExample(typeof(DishDTO), typeof(DishRequestExampleWithIdWithoutEtag))]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> PutDish(
        [FromRoute] Guid id,
        [FromBody] DishDTO dto,
        [FromServices] ICommandProcessor<UpdateDishCommand, DishDTO> processor
    )
    {
        if (string.IsNullOrEmpty(context.IfMatchHeader))
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        var command = new UpdateDishCommand(id, context.IfMatchHeader, dto);
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
    /// Deletes a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to delete.</param>
    /// <param name="processor"></param>
    /// <response code="202">Dish Deleted</response>
    /// <response code="404">Dish Not Found</response> 
    [HttpDelete("{id}", Name = nameof(DeleteDish))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> DeleteDish(
        [FromRoute] Guid id,
        [FromServices] ICommandProcessor<DeleteDishCommand, DishDTO> processor
    )
    {
        var command = new DeleteDishCommand(id);
        var result = await processor.Process(command);

        if (!result.Succeeded)
            return NotFound($"Dish with id does not exist.");

        return Ok(result.Result);
    }

    /// <summary>
    /// Retrieves the image of a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to retrieve the image from.</param>
    /// <param name="responder"></param>
    /// <response code="200">Image Set</response>
    /// <response code="204">No Image</response>
    /// <response code="404">Dish Not Found</response>
    [HttpGet("{id}/image", Name = nameof(GetDishImage))]
    [Produces("image/jpeg", "image/png")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDishImage(
        [FromRoute] Guid id,
        [FromServices] IQueryResponder<DishImageQuery, (DishDTO?, Image?)> responder
    )
    {
        var query = new DishImageQuery(id);
        var (dto, image) = await responder.RespondTo(query);

        if (dto == null) return NotFound();
        if (image == null) return NoContent();

        return File(image.Content, image.ContentType);
    }

    /// <summary>
    /// Sets the image of a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to attach the image to.</param>
    /// <param name="image">The image to attach.</param>
    /// <param name="processor"></param>
    /// <response code="201">Image Set</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Dish Not Found</response>
    /// <response code="415">Image Type Not Supported</response>
    [HttpPut("{id}/image", Name = nameof(PutDishImage))]
    [Consumes("image/jpeg", "image/png")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [SwaggerResponseExample(201, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult> PutDishImage(
        [FromRoute] Guid id,
        [FromBody] Image image,
        [FromServices] ICommandProcessor<UpdateDishImageCommand, DishDTO> processor
    )
    {
        var command = new UpdateDishImageCommand(id, image);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound))
            return NotFound();

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest();

        var actionLink = ActionLink(action: nameof(GetDishImage), values: new { id });
        if (actionLink != null)
            HttpContext.Response.Headers.Location = actionLink;

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Deletes the image of a specific dish.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="processor"></param>
    /// <response code="200">Image Deleted</response>
    /// <response code="204">No Image</response>
    /// <response code="404">Dish Not Found</response>
    [HttpDelete("{id}/image")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDishImage(
        [FromRoute] Guid id,
        [FromServices] ICommandProcessor<DeleteDishImageCommand, DishDTO> processor
    )
    {
        var command = new DeleteDishImageCommand(id);
        var result = await processor.Process(command);

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound && e.Message.Contains("Dish")))
            return NotFound();

        if (result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotFound && e.Message.Contains("Image")))
            return NoContent();

        return Ok();
    }

    private string? ActionLink(string action, object? values)
    {
        if (Url == null)
            return null;

        return Url.ActionLink(
            action: action,
            controller: null,
            values: values,
            protocol: context.Scheme,
            host: context.Host
        );
    }
}
