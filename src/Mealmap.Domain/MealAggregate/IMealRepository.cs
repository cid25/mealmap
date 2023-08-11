﻿using Mealmap.Domain.Common;

namespace Mealmap.Domain.MealAggregate;

public interface IMealRepository : IRepository<Meal>
{
    public IEnumerable<Meal> GetAll(DateOnly? fromDate = null, DateOnly? toDate = null);
}
