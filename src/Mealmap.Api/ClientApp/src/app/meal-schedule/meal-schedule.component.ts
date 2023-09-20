import { Component, OnInit } from '@angular/core';
import { MealService } from '../services/meal.service';
import { DishService } from '../services/dish.service';
import { Meal } from '../interfaces/meal';

@Component({
  selector: 'app-meal-schedule',
  templateUrl: './meal-schedule.component.html',
  styleUrls: ['./meal-schedule.component.css']
})
export class MealScheduleComponent implements OnInit {
  private meals: Meal[] = [];

  private timerange: Timerange = Timerange.Weekly;

  constructor(
    private mealService: MealService,
    private dishService: DishService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.retrieveMeals(
      new Date('2020-01-01T00:00:00.000Z'),
      new Date('2020-01-07T00:00:00.000Z')
    );
    await this.retrieveDishesForMeals();
  }

  mealsForDisplay() {
    return this.meals.sort(
      (a: Meal, b: Meal) => a.diningDate.getTime() - b.diningDate.getTime()
    );
  }

  weeklyRange(): boolean {
    return this.timerange == Timerange.Weekly;
  }

  monthlyRange(): boolean {
    return this.timerange == Timerange.Monthly;
  }

  viewWeeklyRange(): void {
    console.log('weekly');
    if (this.timerange != Timerange.Weekly) {
      this.timerange = Timerange.Monthly;
    }
  }

  viewMonthlyRange(): void {
    console.log('monthly');
    if (this.timerange != Timerange.Monthly) {
      this.timerange = Timerange.Weekly;
    }
  }

  private async retrieveMeals(from: Date, to: Date): Promise<void> {
    this.meals = await this.mealService.getMealsFor(from, to);
  }

  private async retrieveDishesForMeals(): Promise<void> {
    const ids = this.collectDishIds();
    const dishes = await this.dishService.getDishes(ids);

    const meals = this.meals.map((meal) => {
      meal.courses.forEach((course) => {
        const dish = dishes.find((dish) => dish.id === course.dishId);
        if (dish !== undefined) {
          course.dish = dish;
        }
      });
      return meal;
    });

    this.meals = meals;
  }

  private collectDishIds(): string[] {
    return this.meals
      .map((meal) => meal.courses)
      .reduce((accumulated, courses) => accumulated.concat(courses), [])
      .map((course) => course.dishId);
  }
}

enum Timerange {
  Weekly = 1,
  Monthly = 2
}
