import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import { IDish } from '../interfaces/IDish';

@Injectable({
  providedIn: 'root'
})
export class DishService {
  private readonly base_url = 'api/dishes';

  private dishes: Map<string, IDish> = new Map<string, IDish>();

  constructor(
    private http: HttpClient,
    private sanitizer: DomSanitizer
  ) {}

  async getDishes(ids: string[]): Promise<IDish[]> {
    const uniqueIds = Array.from(new Set(ids));

    const missingIds = uniqueIds.filter((id) => !this.dishes.has(id));
    if (missingIds.length > 0) {
      const fetchOperations: Promise<unknown>[] = [];
      for (const id of missingIds) fetchOperations.push(this.fetchDishAndImage(id));
      await Promise.all(fetchOperations);
    }

    const result = uniqueIds
      .map<IDish | undefined>((id) => this.dishes.get(id))
      .filter((dish): dish is IDish => !!dish) as IDish[];
    return result;
  }

  async getDish(id: string): Promise<IDish | null> {
    if (!this.dishes.has(id)) {
      const dish = await this.fetchDishAndImage(id);
      return dish;
    } else {
      const dish = this.dishes.get(id);
      if (dish !== undefined) return dish;
    }

    return null;
  }

  private async fetchDishAndImage(id: string): Promise<IDish> {
    const dishRequest = this.requestDish(id);
    const imageRequest = this.requestImage(id);

    // eslint-disable-next-line prefer-const
    let [dish, imageResponse] = await Promise.all([dishRequest, imageRequest]);

    dish = this.setImageFromResponse(dish, imageResponse);
    this.dishes.set(id, dish);

    return dish;
  }

  private async requestDish(id: string): Promise<IDish> {
    const dish_url = `${this.base_url}/${id}`;
    return firstValueFrom(this.http.get<IDish>(dish_url));
  }

  private requestImage(id: string): Promise<HttpResponse<Blob>> {
    const image_url = `${this.base_url}/${id}/image`;
    return firstValueFrom(this.http.get(image_url, { observe: 'response', responseType: 'blob' }));
  }

  private setImageFromResponse(dish: IDish, response: HttpResponse<Blob>): IDish {
    if (response.status == 200 && response.body) {
      dish.image = new Blob([response.body], {
        type: response.headers.get('Content-Type') ?? ''
      });
      const url = URL.createObjectURL(dish.image);
      dish.localImageURL = this.sanitizer.bypassSecurityTrustUrl(url);
    }
    return dish;
  }
}
