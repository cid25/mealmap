using Mealmap.Api.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<DishDTO> GetDishes()
        {
            return new DishDTO();
        }
    }
}
