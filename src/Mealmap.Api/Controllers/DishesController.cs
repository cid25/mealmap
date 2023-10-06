using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.Queries;
using Mealmap.Api.RequestFormatters;
using Mealmap.Api.Swagger;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class DishesController : ControllerBase
{
    private readonly ILogger<DishesController> _logger;
    private readonly IRepository<Dish> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;

    public DishesController(
        ILogger<DishesController> logger,
        IRepository<Dish> repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<DishDTO, Dish> outputMapper,
        IRequestContext context,
        IMediator mediator)
    {
        _logger = logger;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _context = context;
        _mediator = mediator;
    }

    /// <summary>
    /// Lists dishes.
    /// </summary>
    /// <response code="200">Dishes Returned</response>
    /// <response code="400">Bad Request</response>
    [HttpGet(Name = nameof(GetDishes))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaginatedDTO<DishDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedDTO<DishDTO>>> GetDishes([FromQuery] Guid? next, [FromQuery] int? limit, [FromQuery] string? search)
    {
        if (limit < 1) return BadRequest("Limit must be greater than 0.");

        DishQuery query = new()
        {
            Limit = limit,
            Next = next,
            Searchterm = search == String.Empty ? null : search
        };

        var dto = await _mediator.Send(query);

        return new OkObjectResult(dto);
    }

    /// <summary>
    /// Retrieves a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to retrieve.</param>
    /// <response code="200">Dish Returned</response>
    /// <response code="404">Dish Not Found</response>
    [HttpGet("{id}", Name = nameof(GetDish))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public ActionResult<DishDTO> GetDish([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
        {
            _logger.LogInformation("Attempt to retrieve non-existing dish");
            return NotFound();
        }

        var dto = _outputMapper.FromEntity(dish);
        return dto;
    }

    /// <summary>
    /// Creates a dish.
    /// </summary>
    /// <param name="dto"></param>
    /// <response code="201">Dish Created</response>
    /// <response code="400">Bad Request</response>
    [HttpPost(Name = nameof(PostDish))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(DishDTO), typeof(DishRequestExampleWithoutIdAndEtag))]
    [SwaggerResponseExample(201, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> PostDish([FromBody] DishDTO dto)
    {
        var command = new CreateDishCommand(dto);
        var result = await _mediator.Send(command);

        if (!result.Succeeded && result.Errors.Any(e => e.ErrorCode == CommandErrorCodes.NotValid))
            return BadRequest(String.Join(", ", result.Errors.Where(e => e.ErrorCode == CommandErrorCodes.NotValid).Select(er => er.Message)));

        return CreatedAtAction(nameof(GetDish), new { id = result.Result!.Id }, result.Result);
    }

    /// <summary>
    /// Updates a specific dish.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
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
    public async Task<ActionResult<DishDTO>> PutDish([FromRoute] Guid id, [FromBody] DishDTO dto)
    {
        if (String.IsNullOrEmpty(_context.IfMatchHeader))
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        var command = new UpdateDishCommand(id, _context.IfMatchHeader, dto);
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
    /// Deletes a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to delete.</param>
    /// <response code="202">Dish Deleted</response>
    /// <response code="404">Dish Not Found</response> 
    [HttpDelete("{id}", Name = nameof(DeleteDish))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public async Task<ActionResult<DishDTO>> DeleteDish([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
            return NotFound($"Dish with id does not exist.");

        _repository.Remove(dish);
        await _unitOfWork.SaveTransactionAsync();

        var dto = _outputMapper.FromEntity(dish);
        return Ok(dto);
    }

    /// <summary>
    /// Sets the image of a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to attach the image to.</param>
    /// <param name="image">The image to attach.</param>
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
    public async Task<ActionResult> PutDishImage([FromRoute] Guid id, [FromBody] Image image)
    {
        var dish = _repository.GetSingleById(id);
        if (dish == null)
            return NotFound();

        dish.SetImage(image.Content, image.ContentType);
        _repository.Update(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException)
        {
            BadRequest();
        }

        var actionLink = ActionLink(action: nameof(GetDishImage), values: new { id });
        if (actionLink != null)
            HttpContext.Response.Headers.Location = actionLink;

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Retrieves the image of a specific dish.
    /// </summary>
    /// <param name="id">The id of the dish to retrieve the image from.</param>
    /// <response code="200">Image Set</response>
    /// <response code="204">No Image</response>
    /// <response code="404">Dish Not Found</response>
    [HttpGet("{id}/image", Name = nameof(GetDishImage))]
    [Produces("image/jpeg", "image/png")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult GetDishImage([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
            return NotFound();
        if (dish.Image == null)
            return NoContent();

        return File(dish.Image.Content, dish.Image.ContentType);
    }

    /// <summary>
    /// Deletes the image of a specific dish.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Image Deleted</response>
    /// <response code="204">No Image</response>
    /// <response code="404">Dish Not Found</response>
    [HttpDelete("{id}/image")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDishImage([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
            return NotFound();

        if (dish.Image == null)
            return NoContent();

        dish.RemoveImage();
        _repository.Update(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException)
        {
            BadRequest();
        }

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
            protocol: _context.Scheme,
            host: _context.Host
        );
    }
}
