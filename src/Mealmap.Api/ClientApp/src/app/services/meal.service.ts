import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Meal } from '../interfaces/meal';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private meals: Map<string, Meal> = new Map<string, Meal>();

  constructor(private http: HttpClient) {}

  async getMealsFor(from: Date, to: Date): Promise<Meal[]> {
    const dates: Date[] = this.datesForRange(from, to);

    const initiallyBlankDates = this.datesWithoutMeal(dates);
    const [lowerBound, upperBound] = this.boundsForRange(initiallyBlankDates);
    await this.fetchForRange(lowerBound, upperBound);

    const remainingBlankDates = this.datesWithoutMeal(dates);
    remainingBlankDates.forEach((date) => {
      const meal: Meal = {
        id: '',
        diningDate: date,
        courses: []
      };
      this.meals.set(this.toIsoDateString(date), meal);
    });

    const result: Meal[] = [];
    dates.forEach((date) => {
      const meal = this.meals.get(this.toIsoDateString(date));
      if (meal !== undefined) result.push(meal);
    });

    return result;
  }

  private datesWithoutMeal(dates: Date[]): Date[] {
    return dates.filter((date) => !this.meals.has(this.toIsoDateString(date)));
  }

  private datesForRange(from: Date, to: Date): Date[] {
    const result: Date[] = [];

    let dateMilliseconds = from.getTime();
    while (dateMilliseconds <= to.getTime()) {
      result.push(new Date(dateMilliseconds));
      dateMilliseconds = dateMilliseconds + 24 * 60 * 60 * 1000;
    }

    return result;
  }

  private boundsForRange(dates: Date[]): [Date, Date] {
    const sorted = dates.sort((a, b) => a.getTime() - b.getTime());
    return [sorted[0], sorted[sorted.length - 1]];
  }

  private async fetchForRange(from: Date, to: Date): Promise<void> {
    const url = 'api/meals';
    const options = {
      params: new HttpParams()
        .set('fromDate', this.toIsoDateString(from))
        .set('toDate', this.toIsoDateString(to))
    };
    const mealsRaw = await firstValueFrom(this.http.get<Meal[]>(url, options));

    if (mealsRaw) {
      const mealsTypeCorrected = mealsRaw.map((meal) => {
        const dateFromString = new Date(meal.diningDate);
        dateFromString.setUTCHours(0);
        meal.diningDate = dateFromString;
        return meal;
      });
      mealsTypeCorrected.forEach((meal) =>
        this.meals.set(this.toIsoDateString(meal.diningDate), meal)
      );
    }
  }

  private toIsoDateString(date: Date): string {
    return `
      ${date.getUTCFullYear()}-
      ${('0' + (date.getUTCMonth().toString() + 1)).slice(-2)}-
      ${('0' + date.getUTCDate().toString()).slice(-2)}`;
  }
}
