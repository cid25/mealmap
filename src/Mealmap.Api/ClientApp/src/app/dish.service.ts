import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { Dish } from './dish';

@Injectable({
  providedIn: 'root'
})
export class DishService {

  private dishes: Map<string, Dish> = new Map<string, Dish>();

  constructor(private http: HttpClient) {}

  async getDishes(ids: string[]): Promise<Dish[]> {
    const uniqueDishes = Array.from(new Set(ids));

    for await (const id of uniqueDishes)
      await this.fetchDish(id);

    let result = uniqueDishes.map<Dish|undefined>(id =>
      this.dishes.get(id)).filter((dish): dish is Dish => !!dish) as Dish[];

    return result;
  }

  private async fetchDish(id: string): Promise<void> {
    const base_url = 'api/dishes';

    if (!this.dishes.has(id)) {
      const url = `${base_url}/${id}`;
      const dish = await firstValueFrom(this.http.get<Dish>(url));
      this.dishes.set(id, dish);
    }
  }

  async getDish(id: string): Promise<Dish|null> {
    if (!this.dishes.has(id))
      await this.fetchDish(id);

    const dish = this.dishes.get(id);
    if (dish !== undefined)
      return dish;

    return null;
  }
}
