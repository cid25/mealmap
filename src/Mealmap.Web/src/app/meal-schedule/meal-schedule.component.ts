import { Component, OnInit } from '@angular/core';
import { MealService } from '../services/meal.service';
import { DishService } from '../services/dish.service';
import { Meal } from '../classes/meal';
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
    await this.retrieveMeals();
  }

  mealsForDisplay(): Meal[] {
    return this.meals.sort((a: Meal, b: Meal) => a.diningDate.getTime() - b.diningDate.getTime());
  }

  isWeeklyRange(): boolean {
    return this.timeperiodType == Timeperiod.Weekly;
  }

  isMonthlyRange(): boolean {
    return this.timeperiodType == Timeperiod.Monthly;
  }

  isCurrent(): boolean {
    return DateTime.fromJSDate(this.reference).equals(DateTime.fromJSDate(this.today()));
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
    await this.retrieveMeals();
  }

  async viewMonthlyRange(): Promise<void> {
    if (this.timeperiodType != Timeperiod.Monthly) {
      this.timeperiodType = Timeperiod.Monthly;
    }
    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMeals();
  }

  async shiftTimeperiodPrior(): Promise<void> {
    let reference = DateTime.fromJSDate(this.reference);
    if (this.timeperiodType == Timeperiod.Weekly) reference = reference.minus({ week: 1 });
    else reference = reference.minus({ month: 1 });
    this.reference = reference.toJSDate();

    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMeals();
  }

  async shiftTimeperiodAfter(): Promise<void> {
    let reference = DateTime.fromJSDate(this.reference);
    if (this.timeperiodType == Timeperiod.Weekly) reference = reference.plus({ week: 1 });
    if (this.timeperiodType == Timeperiod.Monthly) reference = reference.plus({ month: 1 });
    this.reference = reference.toJSDate();

    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMeals();
  }

  async deleteMeal(date: Date): Promise<void> {
    await this.mealService.deleteMeal(date);
    await this.retrieveMeals();
  }

  async shiftCurrent(): Promise<void> {
    this.reference = new Date(Date.now());
    this.reference.setUTCHours(0, 0, 0, 0);
    [this.start, this.end] = this.calcTimerange();
    await this.retrieveMeals();
  }

  private async retrieveMeals(): Promise<void> {
    this.meals = await this.mealService.getMealsFor(this.start, this.end);
  }

  private today(): Date {
    const today = new Date(Date.now());
    today.setUTCHours(0, 0, 0, 0);
    return today;
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
