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
    if (initiallyBlankDates.length > 0) {
      const [lowerBound, upperBound] = this.boundsForRange(initiallyBlankDates);
      await this.fetchForRange(lowerBound, upperBound);
    }

    this.fillBlanksWithStubs(dates);

    const result: Meal[] = [];
    dates.forEach((date) => {
      const meal = this.meals.get(this.toIsoDateString(date));
      if (meal !== undefined) result.push(meal);
    });

    return result;
  }

  async getMealFor(date: Date): Promise<Meal> {
    const dateAsString = this.toIsoDateString(date);
    if (!this.meals.has(dateAsString)) await this.fetchForRange(date, date);

    const meal = this.meals.get(dateAsString);
    if (meal == undefined) throw new Error(`meal for ${dateAsString} not found`);

    return meal;
  }

  async deleteMeal(date: Date): Promise<void> {
    const dateString = this.toIsoDateString(date);

    const url = `api/meals/${this.meals.get(dateString)?.id}`;
    await firstValueFrom(this.http.delete(url));

    this.meals.delete(dateString);
  }

  private datesWithoutMeal(dates: Date[]): Date[] {
    return dates.filter((date) => {
      const key = this.toIsoDateString(date);
      if (!this.meals.has(key) || this.meals.get(key)?.id == '') return true;
      else return false;
    });
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

  private fillBlanksWithStubs(dates: Date[]): void {
    const remainingBlankDates = this.datesWithoutMeal(dates);
    remainingBlankDates.forEach((date) => {
      const meal: Meal = {
        id: '',
        eTag: '',
        diningDate: date,
        courses: []
      };
      this.meals.set(this.toIsoDateString(date), meal);
    });
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
    const dateString =
      date.getUTCFullYear() +
      '-' +
      ('0' + (date.getUTCMonth() + 1).toString()).slice(-2) +
      '-' +
      ('0' + date.getUTCDate().toString()).slice(-2);

    return dateString;
  }
}
