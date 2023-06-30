using System.ComponentModel;
using Mealmap.Model;


namespace Mealmap.Api.Tests
{
    internal class FakeMealRepository : Dictionary<Guid,Meal>, IMealRepository
    { 
        public void Create(Meal meal)
        {
            Add(meal.Id, meal);
        }

        public Meal? GetById(Guid id)
        {
            Meal? meal;
            TryGetValue(id, out meal);

            return meal;
        }
    }
}
