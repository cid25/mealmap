import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { DateTime } from 'luxon';
import { IMeal } from '../interfaces/IMeal';
import { Meal } from '../classes/Meal';
import { DishService } from './dish.service';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private readonly base_url = 'api/meals';

  private meals: Map<string, Meal> = new Map<string, Meal>();

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
      const meal = this.meals.get(key);
      if (meal !== undefined) result.push(meal);
    });

    return result;
  }

  async getMealFor(date: Date): Promise<Meal> {
    let result: Meal = new Meal(date);

    const dateKey = Meal.keyFor(date);
    if (this.meals.has(dateKey)) {
      result = this.meals.get(dateKey) as Meal;
    } else {
      const meal = (await this.fetchForRange(date, date))[0];

      if (meal !== undefined) result = meal;
    }

    this.meals.set(result.key(), result);
    await this.retrieveDishesForMeals();
    return result;
  }

  async deleteMeal(date: Date): Promise<void> {
    const dateKey = Meal.keyFor(date);

    const url = `${this.base_url}/${this.meals.get(dateKey)?.id}`;
    await firstValueFrom(this.http.delete(url));

    this.meals.delete(dateKey);
  }

  async saveMeal(meal: Meal): Promise<void> {
    let returned: IMeal | undefined = undefined;
    if (meal.id) {
      const url = `${this.base_url}/${meal.id}`;
      const options = {
        headers: new HttpHeaders()
          .set('If-Match', meal.eTag!)
          .set('Content-Type', 'application/json')
      };
      returned = await firstValueFrom(this.http.put<IMeal>(url, meal.toJSON(), options));
    } else {
      const options = {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      };
      returned = await firstValueFrom(this.http.post<IMeal>(this.base_url, meal.toJSON(), options));
    }

    if (returned !== undefined) {
      const received = Meal.from(returned);
      this.meals.set(received.key(), received);
    }
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

  private async fetchForRange(from: Date, to: Date): Promise<Meal[]> {
    const options = {
      params: new HttpParams().set('fromDate', Meal.keyFor(from)).set('toDate', Meal.keyFor(to))
    };
    const mealData = await firstValueFrom(this.http.get<IMeal[]>(this.base_url, options));

    if (mealData) {
      const meals = mealData.map((rawMeal) => Meal.from(rawMeal));
      meals.forEach((meal) => this.meals.set(meal.key(), meal));
      return meals;
    }
    return [];
  }

  private async retrieveDishesForMeals(): Promise<void> {
    const ids = this.collectMissingDishIds();
    const dishes = await this.dishService.getDishes(ids);

    this.meals.forEach((value) =>
      value.courses.forEach((course) => {
        if (!course.dish) {
          const dish = dishes.find((dish) => dish.id === course.dishId);
          if (dish !== undefined) course.dish = dish;
        }
      })
    );
  }

  private collectMissingDishIds(): string[] {
    const result: string[] = [];
    this.meals.forEach((value) =>
      value.courses.filter((course) => !course.dish).forEach((course) => result.push(course.dishId))
    );
    return result;
  }
}
