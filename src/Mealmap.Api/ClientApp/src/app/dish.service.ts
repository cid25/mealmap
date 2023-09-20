import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import { Dish } from './dish';

@Injectable({
  providedIn: 'root'
})
export class DishService {

  private readonly base_url = 'api/dishes';

  private dishes: Map<string, Dish> = new Map<string, Dish>();

  constructor(
    private http: HttpClient,
    private sanitizer: DomSanitizer
  ) {}

  async getDishes(ids: string[]): Promise<Dish[]> {
    const uniqueIds = Array.from(new Set(ids));

    const missingIds = uniqueIds.filter(id => !this.dishes.has(id));
    if (missingIds.length > 0) {
      let fetchOperations: Promise<any>[] = [];
      for (const id of missingIds)
        fetchOperations.push(this.fetchDishAndImage(id));
      await Promise.all(fetchOperations);
    }

    const result = uniqueIds.map<Dish|undefined>(id =>
      this.dishes.get(id)).filter((dish): dish is Dish => !!dish) as Dish[];
    return result;
  }

  async getDish(id: string): Promise<Dish|null> {
    if (!this.dishes.has(id)) {
      const dish = await this.fetchDishAndImage(id);
      return dish;
    }
    else {
      const dish = this.dishes.get(id);
      if (dish !== undefined)
      return dish;
    }

    return null;
  }

  private async fetchDishAndImage(id: string): Promise<Dish> {
    const dishRequest = this.requestDish(id);
    const imageRequest = this.requestImage(id);

    let [dish, imageResponse] = await Promise.all([dishRequest, imageRequest]);

    dish = this.setImageFromResponse(dish, imageResponse);
    this.dishes.set(id, dish);

    return dish;
  }

  private async requestDish(id: string): Promise<Dish> {
    const dish_url = `${this.base_url}/${id}`;
    return firstValueFrom(this.http.get<Dish>(dish_url));
  }

  private requestImage(id: string): Promise<HttpResponse<Blob>> {
    const image_url = `${this.base_url}/${id}/image`;
    return firstValueFrom(this.http.get(image_url, {observe: 'response', responseType: 'blob'}));
  }

  private setImageFromResponse(dish: Dish, response: HttpResponse<Blob>): Dish {
    if (response.status == 200 && response.body) {
      dish.image = new Blob([response.body], {type: response.headers.get('Content-Type') ?? ''});
      const url = URL.createObjectURL(dish.image);
      dish.localImageURL = this.sanitizer.bypassSecurityTrustUrl(url);
    }
    return dish;
  }
}
