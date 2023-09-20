import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { Dish } from './dish';

@Injectable({
  providedIn: 'root'
})
export class DishService {

  private readonly base_url = 'api/dishes';

  private dishes: Map<string, Dish> = new Map<string, Dish>();

  constructor(private http: HttpClient) {}

  async getDishes(ids: string[]): Promise<Dish[]> {
    const uniqueDishes = Array.from(new Set(ids));


    let fetchOperations: Promise<any>[] = [];
    for (const id of uniqueDishes)
      fetchOperations.push(this.fetchDishAndImage(id));
    await Promise.all(fetchOperations);

    const result = uniqueDishes.map<Dish|undefined>(id =>
      this.dishes.get(id)).filter((dish): dish is Dish => !!dish) as Dish[];

    return result;
  }

  async getDish(id: string): Promise<Dish|null> {
    if (!this.dishes.has(id))
      await this.fetchDishAndImage(id);

    const dish = this.dishes.get(id);
    if (dish !== undefined)
      return dish;

    return null;
  }

  private async fetchDishAndImage(id: string): Promise<void> {

    if (!this.dishes.has(id)) {
      const dishRequest = this.fetchDish(id);
      const imageRequest = this.fetchImage(id);

      const [dish, imageResponse] = await Promise.all([dishRequest, imageRequest]);

      if (imageResponse.status == 200 && imageResponse.body)
        dish.image = new Blob([imageResponse.body], {type: imageResponse.headers.get('Content-Type') ?? ''});
      this.dishes.set(id, dish);
    }
  }

  private async fetchDish(id: string): Promise<Dish> {
    const dish_url = `${this.base_url}/${id}`;
    return firstValueFrom(this.http.get<Dish>(dish_url));
  }

  private fetchImage(id: string): Promise<HttpResponse<Blob>> {
    const image_url = `${this.base_url}/${id}/image`;
    return firstValueFrom(this.http.get(image_url, {observe: 'response', responseType: 'blob'}));
  }
}
