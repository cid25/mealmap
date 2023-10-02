import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { firstValueFrom, catchError, of } from 'rxjs';
import { DishDTO } from '../interfaces/dish.dto';
import { Dish } from '../classes/dish';

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

  async listDishes(): Promise<Dish[]> {
    const dishDTOs = await firstValueFrom(this.http.get<DishDTO[]>(this.base_url));
    const dishes = dishDTOs.map((dto) => Dish.from(dto));
    dishes.forEach((dish) => this.dishes.set(dish.id!, dish));
    this.fetchImages(dishes);
    return dishes;
  }

  async getDishes(ids: string[]): Promise<Dish[]> {
    const uniqueIds = Array.from(new Set(ids));

    const missingIds = uniqueIds.filter((id) => !this.dishes.has(id));
    if (missingIds.length > 0) {
      const fetchOperations: Promise<unknown>[] = [];
      for (const id of missingIds) fetchOperations.push(this.fetchDishAndImage(id));
      await Promise.all(fetchOperations);
    }

    const result = uniqueIds
      .map<Dish | undefined>((id) => this.dishes.get(id))
      .filter((dish): dish is Dish => !!dish) as Dish[];
    return result;
  }

  async getDish(id: string): Promise<Dish | undefined> {
    if (!this.dishes.has(id)) {
      const dish = await this.fetchDishAndImage(id);
      return dish;
    } else {
      const dish = this.dishes.get(id);
      if (dish !== undefined) return dish;
    }

    return undefined;
  }

  getURLforImage(image: Blob): SafeUrl {
    const url = URL.createObjectURL(image);
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  setImageFromFile(dish: Dish, image: Blob): Dish {
    dish.image = image;
    dish.localImageURL = this.getURLforImage(image);
    return dish;
  }

  private async fetchImages(dishes: Dish[]): Promise<void> {
    dishes
      .filter((dish) => !!dish.imageUrl)
      .forEach(async (dish) => {
        const imageResponse = await this.requestImage(dish.id!);
        if (imageResponse !== null) this.setImageFromResponse(dish, imageResponse);
      });
  }

  private async fetchDishAndImage(id: string): Promise<Dish> {
    const dishRequest = this.requestDish(id);
    const imageRequest = this.requestImage(id);

    // eslint-disable-next-line prefer-const
    let [dish, imageResponse] = await Promise.all([dishRequest, imageRequest]);

    if (imageResponse) dish = this.setImageFromResponse(dish, imageResponse);
    this.dishes.set(id, dish);

    return dish;
  }

  private async requestDish(id: string): Promise<Dish> {
    const dish_url = `${this.base_url}/${id}`;
    const dto = await firstValueFrom(this.http.get<DishDTO>(dish_url));
    return Dish.from(dto);
  }

  private requestImage(id: string): Promise<HttpResponse<Blob> | null> {
    const image_url = `${this.base_url}/${id}/image`;
    return firstValueFrom(
      this.http.get(image_url, { observe: 'response', responseType: 'blob' }).pipe(
        catchError((err) => {
          console.log(err);
          return of(null);
        })
      ),
      {
        defaultValue: null
      }
    );
  }

  private setImageFromResponse(dish: Dish, response: HttpResponse<Blob>): Dish {
    if (response.status == 200 && response.body) {
      dish.image = new Blob([response.body], {
        type: response.headers.get('Content-Type') ?? ''
      });
      dish.localImageURL = this.getURLforImage(dish.image);
    }
    return dish;
  }
}
