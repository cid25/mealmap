import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { DateTime } from 'luxon';
import { IMeal } from '../interfaces/IMeal';
import { Meal } from '../classes/Meal';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private readonly base_url = 'api/meals';

  private meals: Map<string, Meal> = new Map<string, Meal>();

  constructor(private http: HttpClient) {}

  async getMealsFor(from: Date, to: Date): Promise<Meal[]> {
    const dates: Date[] = this.datesForRange(from, to);

    const initiallyBlankDates = this.datesWithoutMeal(dates);
    if (initiallyBlankDates.length > 0) {
      const [lowerBound, upperBound] = this.boundsForRange(initiallyBlankDates);
      await this.fetchForRange(lowerBound, upperBound);
    }

    this.fillBlanksWithStubs(dates);

    const result: Meal[] = [];
    dates.forEach((date) => {
      const key = Meal.keyFor(date);
      const meal = this.meals.get(key);
      if (meal !== undefined) result.push(meal);
    });

    return result;
  }

  async getMealFor(date: Date): Promise<Meal> {
    const dateAsString = Meal.keyFor(date);
    if (!this.meals.has(dateAsString)) await this.fetchForRange(date, date);

    const meal = this.meals.get(dateAsString);
    if (meal == undefined) throw new Error(`meal for ${dateAsString} not found`);

    return meal;
  }

  async deleteMeal(date: Date): Promise<void> {
    const dateKey = Meal.keyFor(date);

    const url = `${this.base_url}/${this.meals.get(dateKey)?.id}`;
    await firstValueFrom(this.http.delete(url));

    this.meals.delete(dateKey);
  }

  private datesWithoutMeal(dates: Date[]): Date[] {
    return dates.filter((date) => {
      const key = Meal.keyFor(date);
      if (!this.meals.has(key) || this.meals.get(key)?.id == '') return true;
      else return false;
    });
  }

  private datesForRange(from: Date, to: Date): Date[] {
    const result: Date[] = [];

    let dateIterator = DateTime.fromJSDate(from);
    while (dateIterator.diff(DateTime.fromJSDate(to)).milliseconds <= 0) {
      result.push(dateIterator.toJSDate());
      dateIterator = dateIterator.plus({ day: 1 });
    }

    return result;
  }

  private boundsForRange(dates: Date[]): [Date, Date] {
    const sorted = dates.sort((a, b) => a.getTime() - b.getTime());
    return [sorted[0], sorted[sorted.length - 1]];
  }

  private fillBlanksWithStubs(dates: Date[]): void {
    const remainingBlankDates = this.datesWithoutMeal(dates);
    remainingBlankDates.forEach((date) => {
      const meal = new Meal(date);
      this.meals.set(meal.key(), meal);
    });
  }

  private async fetchForRange(from: Date, to: Date): Promise<void> {
    const options = {
      params: new HttpParams().set('fromDate', Meal.keyFor(from)).set('toDate', Meal.keyFor(to))
    };
    const mealData = await firstValueFrom(this.http.get<IMeal[]>(this.base_url, options));

    if (mealData) {
      const meals = mealData.map((rawMeal) => Meal.from(rawMeal));
      meals.forEach((meal) => this.meals.set(meal.key(), meal));
    }
  }
}
