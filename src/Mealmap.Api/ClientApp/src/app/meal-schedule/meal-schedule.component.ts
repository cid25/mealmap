import { Component, OnInit } from '@angular/core';
import { MealService } from '../meal.service';
import { DishService } from '../dish.service';
import { Meal } from '../meal';
import { Dish } from '../dish';

@Component({
  selector: 'app-meal-schedule',
  templateUrl: './meal-schedule.component.html',
  styleUrls: ['./meal-schedule.component.css']
})
export class MealScheduleComponent {

  private meals: Meal[] = [];

  constructor(
    private mealService: MealService,
    private dishService: DishService) { }

  async ngOnInit(): Promise<void> {
    await this.retrieveMeals(new Date("2020-01-01T00:00:00.000Z"), new Date("2020-01-07T00:00:00.000Z"));
    this.retrieveDishesForMeals();
  }

  mealsForDisplay() {
    return this.meals.sort((a: Meal, b: Meal) =>  a.diningDate.getTime() - b.diningDate.getTime());
  }

  private async retrieveMeals(from: Date, to: Date): Promise<void> {
    this.meals = await this.mealService.getMealsFor(from, to);
  }

  private async retrieveDishesForMeals(): Promise<void> {
    const ids = this.collectDishIds();
    const dishes = await this.dishService.getDishes(ids);

    const meals = this.meals.map(meal => {
      meal.courses.forEach(course => {
        const dish = dishes.find(dish => dish.id === course.dishId);
        if (dish !== undefined) {
          course.dish = dish;
        }
      });
      return meal;
    });

    this.meals = meals;
  }

  private collectDishIds(): string[] {
    return this.meals.map(meal =>
      meal.courses).reduce((accumulated, courses) =>
        accumulated.concat(courses), []).map(course => course.dishId);
  }
}
