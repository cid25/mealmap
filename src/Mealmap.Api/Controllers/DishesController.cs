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

        [HttpGet(Name = nameof(GetDishes))]
        [Produces("application/json")]
        public ActionResult<IEnumerable<DishDTO>> GetDishes()
        {
            var dishes = _repository.GetAll();
            var dishDTOs = _mapper.Map<IEnumerable<Dish>, List<DishDTO>>(dishes);

            return dishDTOs;
        }

        [HttpGet("{id}", Name = nameof(GetDish))]
        [Produces("application/json")]
        public ActionResult<DishDTO> GetDish([FromRoute] Guid id)
        {
            var dish = _repository.GetById(id);

            if (dish == null)
                return NotFound();

            return _mapper.Map<DishDTO>(dish);
        }

        [HttpPost(Name = nameof(PostDish))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public ActionResult<DishDTO> PostDish([FromBody] DishDTO dishDTO)
        {
            if (dishDTO.Id != null || String.IsNullOrWhiteSpace(dishDTO.Name))
                return BadRequest();

            dishDTO = dishDTO with { Id = Guid.NewGuid() };
            var dish = _mapper.Map<Dish>(dishDTO);

            _repository.Create(dish);

            var dishCreated = _mapper.Map<DishDTO>(dish);

            return dishCreated;
        }

    }
}
