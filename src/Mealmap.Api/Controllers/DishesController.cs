using AutoMapper;
using Mealmap.Api.DataTransferObjects;
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

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<DishDTO>> GetDishes()
        {
            var dishes = _repository.GetAll();
            var dishDTOs = _mapper.Map<IEnumerable<Dish>, List<DishDTO>>(dishes);

            return dishDTOs;
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<DishDTO> GetDish([FromRoute] Guid id)
        {
            var dish = _repository.GetById(id);

            if (dish == null)
                return NotFound();

            return _mapper.Map<DishDTO>(dish);
        }

    }
}
