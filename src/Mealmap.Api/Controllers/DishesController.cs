using AutoMapper;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly IDishRepository _repository;
        private readonly IMapper _mapper;

        public DishesController(IDishRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists dishes.
        /// </summary>
        /// <response code="200">Dishes Returned</response>
        [HttpGet(Name = nameof(GetDishes))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<DishDTO>),StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<DishDTO>> GetDishes()
        {
            var dishes = _repository.GetAll();
            var dishDTOs = _mapper.Map<IEnumerable<Dish>, List<DishDTO>>(dishes);

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
                return NotFound();

            return _mapper.Map<DishDTO>(dish);
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
        public ActionResult<DishDTO> PostDish([FromBody] DishDTO dishDTO)
        {
            if (String.IsNullOrWhiteSpace(dishDTO.Name))
                return BadRequest();

            dishDTO = dishDTO with { Id = Guid.NewGuid() };
            var dish = _mapper.Map<Dish>(dishDTO);

            _repository.Create(dish);

            var dishCreated = _mapper.Map<DishDTO>(dish);

            return CreatedAtAction(nameof(GetDish), new { id = dishCreated.Id }, dishCreated );
        }

    }
}
