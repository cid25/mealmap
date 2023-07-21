using Mealmap.Api.DataTransfer;
using Mealmap.Api.Formatters;
using Mealmap.Api.Swashbuckle;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly ILogger<DishesController> _logger;
        private readonly IDishRepository _repository;
        private readonly DishMapper _mapper;

        public DishesController(
            ILogger<DishesController> logger,
            IDishRepository repository,
            DishMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
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
            var dishDTOs = _mapper.MapFromEntities(dishes);

            return dishDTOs;
        }

        /// <summary>
        /// Retrieves a specific dish.
        /// </summary>
        /// <param name="id">The id of the dish.</param>
        /// <response code="200">Dish Returned</response>
        /// <response code="404">Dish Not Found</response>
        [HttpGet("{id}", Name = nameof(GetDish))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DishDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DishDTO> GetDish([FromRoute] Guid id)
        {
            var dish = _repository.GetById(id);

            if (dish == null)
            {
                _logger.LogInformation("Attempt to retrieve non-existing dish");
                return NotFound();
            }

            var dto = _mapper.MapFromEntity(dish);

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(DishDTO), typeof(DishPostRequestExample))]
        [SwaggerResponseExample(201, typeof(DishPostResponseExample))]
        public ActionResult<DishDTO> PostDish([FromBody] DishDTO dishDTO)
        {
            Dish dish;

            try
            {
                dish = _mapper.MapFromDTO(dishDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            _repository.Create(dish);
            _logger.LogInformation("Created dish with id {Id}", dish.Id);

            var dishCreated = _mapper.MapFromEntity(dish);

            return CreatedAtAction(nameof(GetDish), new { id = dishCreated.Id }, dishCreated);
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
        public ActionResult PutDishImage([FromRoute] Guid id, [FromBody] Image image)
        {
            var dish = _repository.GetById(id);
            if (dish == null)
                return NotFound();

            dish.Image = new DishImage(content: image.Content, contentType: image.ContentType);
            _repository.Update(dish);

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
            var dish = _repository.GetById(id);

            if (dish == null)
                return NotFound();
            if (dish.Image == null)
                return NoContent();

            return File(dish.Image.Content, dish.Image.ContentType);
        }

        private string? ActionLink(string action, object? values)
        {
            if (Url == null)
                return null;

            return Url.ActionLink(
                action: action,
                controller: null,
                values: values,
                protocol: HttpContext.Request.Scheme,
                host: HttpContext.Request.Host.Value
            );
        }
    }
}
