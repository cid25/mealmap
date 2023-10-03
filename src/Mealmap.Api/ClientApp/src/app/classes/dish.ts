import { SafeUrl } from '@angular/platform-browser';
import { Ingredient } from './ingredient';
import { DishDTO } from '../interfaces/dish.dto';
import { DishFormData } from '../interfaces/dish-form-data';
import { ETag } from '../classes/etag';

export class Dish {
  id?: string;
  name?: string;
  description?: string;
  servings?: number;
  ingredients?: Ingredient[];
  instructions?: string;
  imageUrl?: string;
  image?: Blob;
  localImageURL?: SafeUrl;

  static from(dto: DishDTO): [Dish, ETag] {
    const result = new Dish();
    return [result.copy(dto), new ETag(dto.eTag)];
  }

  clone(): Dish {
    const result = new Dish();
    result.id = this.id;
    result.name = this.name;
    result.description = this.description;
    result.servings = this.servings;
    result.ingredients = this.ingredients?.map((original) => original.clone());
    result.instructions = this.instructions;
    result.imageUrl = this.imageUrl;
    result.image = this.image;
    result.localImageURL = this.localImageURL;
    return result;
  }

  map(data: DishFormData): Dish {
    this.name = data.name!;
    this.description = data.description;
    this.servings = data.servings;
    this.ingredients = data.ingredients.map((data) => Ingredient.fromFormData(data));
    this.instructions = data.instructions;
    return this;
  }

  toJSON(): string {
    const result = {
      id: this.id ?? null,
      name: this.name ?? null,
      description: this.description ?? null,
      servings: this.servings ?? null,
      ingredients: this.ingredients ?? [],
      instructions: this.instructions ?? null
    };

    return JSON.stringify(result);
  }

  deleteImage(): void {
    this.image = undefined;
    this.localImageURL = undefined;
  }

  setImage(image: Blob | undefined, localImageURL: SafeUrl | undefined): void {
    this.image = image;
    this.localImageURL = localImageURL;
  }

  private copy(dto: DishDTO): Dish {
    this.id = dto.id;
    this.name = dto.name;
    this.description = dto.description;
    this.servings = dto.servings;
    this.ingredients = dto.ingredients.map((data) => Ingredient.fromDTO(data));
    this.instructions = dto.instructions;
    this.imageUrl = dto.imageUrl;
    return this;
  }
}
