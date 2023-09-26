import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MealService } from '../services/meal.service';
import { DishService } from '../services/dish.service';
import { Meal } from '../classes/Meal';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-meal-schedule',
  templateUrl: './meal-schedule.component.html',
  styleUrls: ['./meal-schedule.component.css']
})
export class MealScheduleComponent implements OnInit {
  private meals: Meal[] = [];

  private timeperiodType: Timeperiod;
  private reference: Date;
  private start: Date;
  private end: Date;

  private _dateUnderEdit: Date | undefined = undefined;

  @Output()
  editStarted = new EventEmitter();

  constructor(
    private mealService: MealService,
    private dishService: DishService
  ) {
    this.timeperiodType = Timeperiod.Weekly;

    this.reference = new Date(Date.now());
    this.reference.setUTCHours(0, 0, 0, 0);

    [this.start, this.end] = this.calcTimerange();
  }

  async ngOnInit(): Promise<void> {
    await this.retrieveMealsWithDishes();
  }

  mealsForDisplay(): Meal[] {
    return this.meals.sort((a: Meal, b: Meal) => a.diningDate.getTime() - b.diningDate.getTime());
  }

  weeklyRange(): boolean {
    return this.timeperiodType == Timeperiod.Weekly;
  }

  monthlyRange(): boolean {
    return this.timeperiodType == Timeperiod.Monthly;
  }

  timeLabel(): string {
    const datetime = DateTime.fromJSDate(this.reference);
    if (this.timeperiodType == Timeperiod.Weekly)
      return `CW ${datetime.weekNumber} ${datetime.weekYear}`;
    else return `${datetime.monthShort} ${datetime.year}`;
  }

  async viewWeeklyRange(): Promise<void> {
    if (this.timeperiodType != Timeperiod.Weekly) {
      this.timeperiodType = Timeperiod.Weekly;
    }
    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMealsWithDishes();
  }

  async viewMonthlyRange(): Promise<void> {
    if (this.timeperiodType != Timeperiod.Monthly) {
      this.timeperiodType = Timeperiod.Monthly;
    }
    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMealsWithDishes();
  }

  async shiftTimeperiodPrior(): Promise<void> {
    let reference = DateTime.fromJSDate(this.reference);
    if (this.timeperiodType == Timeperiod.Weekly) reference = reference.minus({ week: 1 });
    else reference = reference.minus({ month: 1 });
    this.reference = reference.toJSDate();

    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMealsWithDishes();
  }

  async shiftTimeperiodAfter(): Promise<void> {
    let reference = DateTime.fromJSDate(this.reference);
    if (this.timeperiodType == Timeperiod.Weekly) reference = reference.plus({ week: 1 });
    if (this.timeperiodType == Timeperiod.Monthly) reference = reference.plus({ month: 1 });
    this.reference = reference.toJSDate();

    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMealsWithDishes();
  }

  startEdit(date: Date): void {
    this._dateUnderEdit = date;
    this.editStarted.emit(date);
  }

  stopEdit(): void {
    this._dateUnderEdit = undefined;
  }

  dateUnderEdit(date: Date): boolean {
    return this._dateUnderEdit?.toISOString() == date.toISOString();
  }

  async deleteMeal(date: Date): Promise<void> {
    await this.mealService.deleteMeal(date);
    await this.retrieveMealsWithDishes();
  }

  private async retrieveMealsWithDishes(): Promise<void> {
    this.meals = await this.mealService.getMealsFor(this.start, this.end);
    await this.retrieveDishesForMeals();
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

  private calcTimerange(): [Date, Date] {
    if (this.timeperiodType == Timeperiod.Weekly) {
      const start = DateTime.fromJSDate(this.reference).startOf('week').toJSDate();
      const end = DateTime.fromJSDate(this.reference)
        .endOf('week')
        .set({ hour: 0, minute: 0, second: 0, millisecond: 0 })
        .toJSDate();
      return [start, end];
    } else {
      const start = DateTime.fromJSDate(this.reference).startOf('month').toJSDate();
      const end = DateTime.fromJSDate(this.reference)
        .endOf('month')
        .set({ hour: 0, minute: 0, second: 0, millisecond: 0 })
        .toJSDate();
      return [start, end];
    }
  }
}

enum Timeperiod {
  Weekly = 1,
  Monthly = 2
}
