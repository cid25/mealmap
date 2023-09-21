import { Component, OnInit, Output, EventEmitter } from '@angular/core';
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

  private timerange: Timerange;

  private start: Date;
  private end: Date;

  @Output()
  editStarted = new EventEmitter();

  constructor(
    private mealService: MealService,
    private dishService: DishService
  ) {
    this.timerange = Timerange.Weekly;

    const now = new Date(Date.now());
    now.setUTCHours(0, 0, 0, 0);
    this.start = this.startOfWeek(now);
    this.end = this.endOfWeek(now);
  }

  async ngOnInit(): Promise<void> {
    await this.retrieveMeals(this.start, this.end);
    await this.retrieveDishesForMeals();
  }

  mealsForDisplay(): Meal[] {
    return this.meals.sort((a: Meal, b: Meal) => a.diningDate.getTime() - b.diningDate.getTime());
  }

  weeklyRange(): boolean {
    return this.timerange == Timerange.Weekly;
  }

  monthlyRange(): boolean {
    return this.timerange == Timerange.Monthly;
  }

  viewWeeklyRange(): void {
    if (this.timerange != Timerange.Weekly) {
      this.timerange = Timerange.Monthly;
    }
  }

  viewMonthlyRange(): void {
    if (this.timerange != Timerange.Monthly) {
      this.timerange = Timerange.Weekly;
    }
  }

  startEdit(date: Date): void {
    this.editStarted.emit(date);
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

  private startOfWeek(date: Date): Date {
    const diff = date.getDate() - date.getDay() + (date.getDay() === 0 ? -6 : 1);
    return new Date(date.setDate(diff));
  }

  private endOfWeek(date: Date): Date {
    const diff = date.getDate() - date.getDay() + (date.getDay() === 0 ? 1 : 7);
    return new Date(date.setDate(diff));
  }
}

enum Timerange {
  Weekly = 1,
  Monthly = 2
}
