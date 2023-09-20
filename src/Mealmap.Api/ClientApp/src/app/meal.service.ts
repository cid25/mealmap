import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { Meal } from './meal';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private meals: Map<String, Meal> = new Map<String, Meal>();

  constructor(private http: HttpClient) { }

  async getMealsFor(from: Date, to: Date): Promise<Meal[]> {
    const dates: Date[] = this.datesForRange(from, to);

    const datesWithoutMeal = this.datesWithoutMeal(dates);

    const bounds = this.boundsForRange(datesWithoutMeal);
    await this.fetchForRange(bounds[0], bounds[1]);

    let result: Meal[] = [];
    dates.forEach(date => {
      const meal = this.meals.get(this.toIsoDateString(date));
      if (meal !== undefined)
        result.push(meal);
    });

    return result;
  }

  private datesWithoutMeal(dates: Date[]): Date[] {
    return dates.filter(date =>
      !this.meals.has(this.toIsoDateString(date)));
  }

  private datesForRange(from: Date, to: Date): Date[] {
    let result: Date[] = [];

    let dateMilliseconds = from.getTime();
    while(dateMilliseconds <= to.getTime()) {
      result.push(new Date(dateMilliseconds));
      dateMilliseconds = dateMilliseconds + (24 * 60 * 60 * 1000);
    }

    return result;
  }

  private boundsForRange(dates: Date[]): [Date, Date] {
    const sorted = dates.sort((a, b) => a.getTime() - b.getTime());
    return [sorted[0], sorted[sorted.length-1]];
  }

  private async fetchForRange(from: Date, to: Date): Promise<void> {
    const url = 'api/meals';
    const options = {
      params: new HttpParams().set('fromDate', this.toIsoDateString(from)).set('toDate', this.toIsoDateString(to))
    };
    const mealsRaw = await firstValueFrom(this.http.get<Meal[]>(url, options));

    if (mealsRaw) {
      const mealsTypeCorrected = mealsRaw.map(meal => {
        const dateFromString = new Date(meal.diningDate);
        dateFromString.setUTCHours(0);
        meal.diningDate = dateFromString;
        return meal; });
      mealsTypeCorrected.forEach(meal => this.meals.set(this.toIsoDateString(meal.diningDate), meal));
    }
  }

  private toIsoDateString(date: Date): string {
    return `${date.getUTCFullYear()}-${('0'+(date.getUTCMonth().toString()+1)).slice(-2)}-${('0'+date.getUTCDate().toString()).slice(-2)}`;
  }
}