using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.InputMappers;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.RequestFormatters;
using Mealmap.Api.Swagger;
using Mealmap.Domain.DishAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DishesController : ControllerBase
{
    private readonly ILogger<DishesController> _logger;
    private readonly IDishRepository _repository;
    private readonly DishInputMapper _inputMapper;
    private readonly DishOutputMapper _outputMapper;
    private readonly IRequestContext _requestContext;

    public DishesController(
        ILogger<DishesController> logger,
        IDishRepository repository,
        DishInputMapper inputMapper,
        DishOutputMapper outputMapper,
        IRequestContext context)
    {
        _logger = logger;
        _repository = repository;
        _inputMapper = inputMapper;
        _outputMapper = outputMapper;
        _requestContext = context;
    }

    /// <summary>
    /// Lists dishes.
    /// </summary>
    /// <response code="200">Dishes Returned</response>
    [HttpGet(Name = nameof(GetDishes))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<DishDTO>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DishDTO>> GetDishes()
    {
        var dishes = _repository.GetAll();
        var dataTransferObjects = _outputMapper.FromEntities(dishes);

        return new OkObjectResult(dataTransferObjects);
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
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
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
    /// <param name="dishDTO"></param>
    /// <response code="201">Dish Created</response>
    /// <response code="400">Bad Request</response>
    [HttpPost(Name = nameof(PostDish))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(DishDTO), typeof(DishRequestExampleWithoutIdAndEtag))]
    [SwaggerResponseExample(201, typeof(DishResponseExampleWithIdAndEtag))]
    public ActionResult<DishDTO> PostDish([FromBody] DishDTO dishDTO)
    {
        if (dishDTO.Id != null && dishDTO.Id != Guid.Empty)
            return BadRequest("Field id is not allowed.");

        Dish dish;

        try
        {
            dish = _inputMapper.FromDataTransferObject(dishDTO);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        _repository.Add(dish);
        _logger.LogInformation("Created dish with id {Id}", dish.Id);

        var dishCreated = _outputMapper.FromEntity(dish);

        return CreatedAtAction(nameof(GetDish), new { id = dishCreated.Id }, dishCreated);
    }

    /// <summary>
    /// Updates an existing dish.
    /// </summary>
    /// <param name="dishDTO"></param>
    /// <response code="200">Dish Updated</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Dish Not Found</response>
    /// <response code="412">ETag Doesn't Match</response>
    /// <response code="428">Update Requires ETag</response>
    [HttpPut(Name = nameof(PutDish))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(void), StatusCodes.Status428PreconditionRequired)]
    [SwaggerRequestExample(typeof(DishDTO), typeof(DishRequestExampleWithIdWithoutEtag))]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public ActionResult<DishDTO> PutDish([FromBody] DishDTO dishDTO)
    {
        if (_requestContext.IfMatchHeader.IsNullOrEmpty())
            return new StatusCodeResult(StatusCodes.Status428PreconditionRequired);

        if (dishDTO.Id == null || dishDTO.Id == Guid.Empty)
            return BadRequest("Field id is mandatory.");

        if (_repository.GetSingleById((Guid)dishDTO.Id) == null)
            return NotFound($"Dish with id {dishDTO.Id} doesn't exist.");

        Dish dish;
        try
        {
            dish = _inputMapper.FromDataTransferObject(dishDTO);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            _repository.Update(dish, retainImage: true);
            _logger.LogInformation("Updated dish with id {Id}", dish.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
        }

        return _outputMapper.FromEntity(dish);
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
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(200, typeof(DishResponseExampleWithIdAndEtag))]
    public ActionResult<DishDTO> DeleteDish([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
            return NotFound($"Dish with id {id} doesn't exist.");

        _repository.Remove(dish);

        var dto = _outputMapper.FromEntity(dish);
        return Ok(dto);
    }

    /// <summary>
    /// Sets the image of a dish.
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
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status415UnsupportedMediaType)]
    [SwaggerResponseExample(201, typeof(DishResponseExampleWithIdAndEtag))]
    public ActionResult PutDishImage([FromRoute] Guid id, [FromBody] Image image)
    {
        var dish = _repository.GetSingleById(id);
        if (dish == null)
            return NotFound();

        dish.Image = new DishImage(content: image.Content, contentType: image.ContentType);
        _repository.Update(dish, retainImage: false);

        var actionLink = ActionLink(action: nameof(GetDishImage), values: new { id });

        if (actionLink != null)
            HttpContext.Response.Headers.Location = actionLink;

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Retrieves the image of a dish.
    /// </summary>
    /// <param name="id">The id of the dish to retrieve the image from.</param>
    /// <response code="201">Image Set</response>
    /// <response code="204">No Image</response>
    /// <response code="404">Dish Not Found</response>
    [HttpGet("{id}/image", Name = nameof(GetDishImage))]
    [Produces("image/jpeg", "image/png")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public ActionResult DeleteDishImage([FromRoute] Guid id)
    {
        var dish = _repository.GetSingleById(id);

        if (dish == null)
            return NotFound();

        if (dish.Image == null)
            return NoContent();

        dish.Image = null;
        _repository.Update(dish, retainImage: false);

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
            protocol: _requestContext.Scheme,
            host: _requestContext.Host
        );
    }
}
