using Mealmap.Model;


namespace Mealmap.Api.UnitTests
{
    internal class FakeDishRepository : Dictionary<Guid,Dish>, IDishRepository
    { 
        public IEnumerable<Dish> GetAll()
        {
            return Values;
        }
    }
}
