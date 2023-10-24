import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpParams } from '@angular/common/http';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { firstValueFrom, catchError, of } from 'rxjs';
import { Paginated } from 'src/app/domain/paginated.dto';
import { DishDTO } from 'src/app/domain/dish.dto';
import { Dish } from 'src/app/domain/dish';
import { ETag } from 'src/app/domain/etag';

@Injectable({
  providedIn: 'root'
})
export class DishService {
  private readonly base_url = 'api/dishes';

  private dishCache: Map<string, Dish> = new Map<string, Dish>();
  private etagCache: Map<string, ETag> = new Map<string, ETag>();

  constructor(
    private http: HttpClient,
    private sanitizer: DomSanitizer
  ) {}

  async get(maxResults?: number, searchterm?: string): Promise<Dish[]> {
    let params = new HttpParams().set(
      'limit',
      maxResults == undefined || maxResults > 50 ? 50 : maxResults
    );
    if (searchterm != undefined) params = params.set('search', searchterm);

    let response = await firstValueFrom(
      this.http.get<Paginated<DishDTO>>(this.base_url, { params: params })
    );

    const dtos = response.items.map((dishDTO) => Dish.from(dishDTO));

    while (response.next != undefined && (maxResults == undefined || dtos.length < maxResults)) {
      response = await firstValueFrom(this.http.get<Paginated<DishDTO>>(response.next.toString()));
      dtos.push(...response.items.map((dishDTO) => Dish.from(dishDTO)));
    }

    dtos.forEach(([dish, etag]) => this.updateCachesWith(dish, etag));

    const dishes = dtos.map(([dish]) => dish);
    await this.loadImagesFor(dishes);

    const result = dishes.map((dish) => dish.clone());
    return result;
  }

  async getByIds(ids: string[]): Promise<Dish[]> {
    const uniqueIds = Array.from(new Set(ids));

    const missingIds = uniqueIds.filter((id) => !this.dishCache.has(id));
    if (missingIds.length > 0) {
      const retrievals = missingIds.map((id) => this.retrieveDishAndImageById(id));
      await Promise.all(retrievals);
    }

    const result = uniqueIds
      .map<Dish | undefined>((id) => this.dishCache.get(id))
      .filter((dish): dish is Dish => !!dish) as Dish[];
    return result.map((dish) => dish.clone());
  }

  async getById(id: string): Promise<Dish | undefined> {
    if (this.dishCache.has(id)) {
      const dish = this.dishCache.get(id);
      if (dish !== undefined) return dish.clone();
    } else {
      const dish = await this.retrieveDishAndImageById(id);
      return dish.clone();
    }

    return undefined;
  }

  async save(dish: Dish): Promise<Dish> {
    if (dish.id) return await this.updateDish(dish);
    else return this.createDish(dish);
  }

  async delete(dish: Dish): Promise<void> {
    await this.deleteDish(dish);
    this.clearCachesOf(dish);
  }

  urlFor(image: Blob): SafeUrl {
    const url = URL.createObjectURL(image);
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  setImageFromBlob(dish: Dish, image: Blob): void {
    dish.image = image;
    dish.localImageURL = this.urlFor(image);
  }

  private async loadImagesFor(dishes: Dish[]): Promise<void> {
    const requests = dishes
      .filter((dish) => !!dish.imageUrl)
      .map(async (dish) => {
        const imageResponse = await this.getImage(dish.id!);
        if (imageResponse !== null) this.setImage(dish, imageResponse);
      });
    await Promise.all(requests);
  }

  private async retrieveDishAndImageById(id: string): Promise<Dish> {
    const dishRequest = this.getDish(id);
    const imageRequest = this.getImage(id);

    const [dto, imageResponse] = await Promise.all([dishRequest, imageRequest]);
    const [result, etag] = Dish.from(dto);

    if (imageResponse) this.setImage(result, imageResponse);
    this.updateCachesWith(result, etag);

    return result;
  }

  private async updateDish(dish: Dish): Promise<Dish> {
    const copyOnRead = this.dishCache.get(dish.id!)!;
    const imageUpdated = dish.image != undefined && dish.image != copyOnRead.image;
    const imageDeleted = dish.image == undefined && copyOnRead.image != undefined;

    const responseDTO = await this.putDish(dish);
    let [result, etag] = Dish.from(responseDTO!);

    if (imageUpdated || imageDeleted) {
      if (imageUpdated) await this.putImage(dish.id!, dish.image!);
      else if (imageDeleted) await this.deleteImage(dish.id!);

      [result, etag] = Dish.from(await this.getDish(dish.id!));
    }

    result.setImage(dish.image, dish.localImageURL);
    this.updateCachesWith(result, etag);
    return result.clone();
  }

  private async createDish(dish: Dish): Promise<Dish> {
    const creationResponse = await this.postDish(dish);
    const [created] = Dish.from(creationResponse!);

    const hasImage = dish.image != undefined;
    if (hasImage) await this.putImage(created.id!, dish.image!);

    const responseDTO = await this.getDish(creationResponse!.id);
    const [result, etag] = Dish.from(responseDTO!);
    if (hasImage) result.setImage(dish.image, dish.localImageURL);

    if (responseDTO !== undefined) this.updateCachesWith(result, etag);

    return result.clone();
  }

  private updateCachesWith(dish: Dish, etag: ETag): void {
    this.dishCache.set(dish.id!, dish);
    this.etagCache.set(dish.id!, etag);
  }

  private clearCachesOf(dish: Dish): void {
    this.dishCache.delete(dish.id!);
    this.etagCache.delete(dish.id!);
  }

  private setImage(dish: Dish, response: HttpResponse<Blob>): Dish {
    if (response.status == 200 && response.body) {
      dish.image = new Blob([response.body], {
        type: response.headers.get('Content-Type') ?? ''
      });
      dish.localImageURL = this.urlFor(dish.image);
    }
    return dish;
  }

  private async getDish(id: string): Promise<DishDTO> {
    const dish_url = `${this.base_url}/${id}`;
    return firstValueFrom(this.http.get<DishDTO>(dish_url));
  }

  private getImage(id: string): Promise<HttpResponse<Blob> | null> {
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

  private async putDish(dish: Dish): Promise<DishDTO | undefined> {
    const url = `${this.base_url}/${dish.id}`;
    const options = {
      headers: new HttpHeaders()
        .set('If-Match', this.etagCache.get(dish.id!)!.toString())
        .set('Content-Type', 'application/json')
    };
    return await firstValueFrom(this.http.put<DishDTO>(url, dish.toJSON(), options));
  }

  private async postDish(dish: Dish): Promise<DishDTO | undefined> {
    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    };
    return await firstValueFrom(this.http.post<DishDTO>(this.base_url, dish.toJSON(), options));
  }

  private async deleteDish(dish: Dish): Promise<void> {
    const url = `${this.base_url}/${dish.id}`;
    await firstValueFrom(this.http.delete(url));
  }

  private async putImage(id: string, image: Blob): Promise<void> {
    const url = `${this.base_url}/${id}/image`;
    const options = {
      headers: new HttpHeaders().set('Content-Type', image!.type)
    };
    await firstValueFrom(this.http.put(url, image, options));
  }

  private async deleteImage(id: string): Promise<void> {
    const url = `${this.base_url}/${id}/image`;
    await firstValueFrom(this.http.delete(url));
  }
}
