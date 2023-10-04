import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { DateTime } from 'luxon';
import { MealDTO } from '../interfaces/meal.dto';
import { Meal } from '../classes/meal';
import { DishService } from './dish.service';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private readonly base_url = 'api/meals';

  private mealCache: Map<string, Meal> = new Map<string, Meal>();

  constructor(
    private http: HttpClient,
    private dishService: DishService
  ) {}

  async getMealsFor(from: Date, to: Date): Promise<Meal[]> {
    const dates: Date[] = this.datesForRange(from, to);

    const initiallyBlankDates = this.datesWithoutMeal(dates);
    if (initiallyBlankDates.length > 0) {
      const [lowerBound, upperBound] = this.boundsForRange(initiallyBlankDates);
      await this.fetchForRange(lowerBound, upperBound);
    }

    await this.retrieveDishesForMeals();

    this.fillBlanksWithStubs(dates);

    const result: Meal[] = [];
    dates.forEach((date) => {
      const key = Meal.keyFor(date);
      const meal = this.mealCache.get(key);
      if (meal !== undefined) result.push(meal);
    });

    return result.map((meal) => meal.clone());
  }

  async getMealFor(date: Date): Promise<Meal> {
    let result: Meal = new Meal(date);

    const dateKey = Meal.keyFor(date);
    if (this.mealCache.has(dateKey)) {
      result = this.mealCache.get(dateKey) as Meal;
    } else {
      const meal = (await this.fetchForRange(date, date))[0];

      if (meal !== undefined) result = meal;
    }

    this.mealCache.set(result.key(), result);
    await this.retrieveDishesForMeals();
    return result.clone();
  }

  async deleteMeal(date: Date): Promise<void> {
    const dateKey = Meal.keyFor(date);

    const url = `${this.base_url}/${this.mealCache.get(dateKey)?.id}`;
    await firstValueFrom(this.http.delete(url));

    this.mealCache.delete(dateKey);
  }

  async saveMeal(meal: Meal): Promise<void> {
    let returned: MealDTO | undefined = undefined;
    if (meal.id) {
      const url = `${this.base_url}/${meal.id}`;
      const options = {
        headers: new HttpHeaders()
          .set('If-Match', meal.eTag!)
          .set('Content-Type', 'application/json')
      };
      returned = await firstValueFrom(this.http.put<MealDTO>(url, meal.toJSON(), options));
    } else {
      const options = {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      };
      returned = await firstValueFrom(
        this.http.post<MealDTO>(this.base_url, meal.toJSON(), options)
      );
    }

    if (returned !== undefined) {
      const received = Meal.from(returned);
      this.mealCache.set(received.key(), received);
    }
  }

  private datesWithoutMeal(dates: Date[]): Date[] {
    return dates.filter((date) => {
      const key = Meal.keyFor(date);
      if (!this.mealCache.has(key) || this.mealCache.get(key)?.id == '') return true;
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
      this.mealCache.set(meal.key(), meal);
    });
  }

  private async fetchForRange(from: Date, to: Date): Promise<Meal[]> {
    const options = {
      params: new HttpParams().set('fromDate', Meal.keyFor(from)).set('toDate', Meal.keyFor(to))
    };
    const mealData = await firstValueFrom(this.http.get<MealDTO[]>(this.base_url, options));

    if (mealData) {
      const meals = mealData.map((rawMeal) => Meal.from(rawMeal));
      meals.forEach((meal) => this.mealCache.set(meal.key(), meal));
      return meals;
    }
    return [];
  }

  private async retrieveDishesForMeals(): Promise<void> {
    const ids = this.collectDishIdsForMeals();
    const dishes = await this.dishService.getByIds(ids);

    this.mealCache.forEach((value) =>
      value.courses.forEach((course) => {
        const dish = dishes.find((dish) => dish.id === course.dishId);
        if (dish !== undefined) course.dish = dish;
      })
    );
  }

  private collectDishIdsForMeals(): string[] {
    const result: string[] = [];
    this.mealCache.forEach((value) => result.push(...value.courses.map((course) => course.dishId)));
    return result;
  }
}
